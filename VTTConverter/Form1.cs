using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using Keys = OpenQA.Selenium.Keys;

namespace VTTConverter
{
    public partial class Form1 : Form
    {
        private IWebDriver _driver;
        private bool _isTranslating = false;
        private List<string> FilesList = new List<string>();
        public Form1()
        {
            InitializeComponent();
        }
        private async void btnTranslate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtVttFile.Text))
            {
                MessageBox.Show("لطفاً یک فایل VTT انتخاب کنید.", "خطا", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (FilesList.Count() == 0)
            {
                MessageBox.Show("فایل ورودی وجود ندارد.", "خطا", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                progressBar1.Maximum = FilesList.Count();
                progressBar1.Value = 0;
                foreach (var item in FilesList)
                {
                    string iinputFile = item;
                    string ioutputFile = Path.ChangeExtension(iinputFile, ".srt");
                    ConvertVttToSrt(iinputFile, ioutputFile);
                    progressBar1.Value++;
                }

                MessageBox.Show($"تبدیل با موفقیت انجام شد.\nفایل خروجی: {txtVttFile.Text}", "موفقیت", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطا در تبدیل فایل: {ex.Message}", "خطا", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "فایل‌های VTT (*.vtt)|*.vtt|تمام فایل‌ها (*.*)|*.*";
                openFileDialog.Title = "انتخاب فایل VTT";
                openFileDialog.Multiselect = true;
                FilesList.Clear();
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    for (int i = 0; i < openFileDialog.FileNames.Count(); i++)
                    {
                        FilesList.Add(openFileDialog.FileNames[i]);
                    }

                    txtVttFile.Text = Path.GetDirectoryName(openFileDialog.FileName);
                }
            }
        }

        private string[] SplitVttContent(string content)
        {
            // تقسیم محتوا به بلوک‌های 10 تایی برای جلوگیری از محدودیت طول پیام
            var blocks = content.Split(new[] { "\n\n" }, StringSplitOptions.RemoveEmptyEntries);
            int chunkSize = 10;
            int chunkCount = (int)Math.Ceiling((double)blocks.Length / chunkSize);

            var chunks = new string[chunkCount];
            for (int i = 0; i < chunkCount; i++)
            {
                int startIndex = i * chunkSize;
                int length = Math.Min(chunkSize, blocks.Length - startIndex);
                chunks[i] = string.Join("\n\n", blocks, startIndex, length);
            }

            return chunks;
        }

        private async Task<string> TranslateWithDeepSeek(string text)
        {
            try
            {
                // رفتن به صفحه چت DeepSeek
                _driver.Navigate().GoToUrl("https://chat.deepseek.com/");

                // انتظار هوشمندانه برای بارگذاری صفحه
                var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(30));

                // اول مطمئن شویم صفحه کامل لود شده
                wait.Until(d => ((IJavaScriptExecutor)d).ExecuteScript("return document.readyState").Equals("complete"));

                // سپس منتظر وجود textarea بمانیم
                var textArea = wait.Until(d =>
                {
                    try
                    {
                        var element = d.FindElement(By.CssSelector("textarea"));
                        return element.Displayed && element.Enabled ? element : null;
                    }
                    catch
                    {
                        return null;
                    }
                });

                // ارسال متن برای ترجمه
                string prompt = $"لطفا متن زیر را به فارسی ترجمه کن و فقط متن ترجمه شده را برگردان:\n\n{text}";
                textArea.Clear();
                textArea.SendKeys(prompt);
                textArea.SendKeys(OpenQA.Selenium.Keys.Enter);

                // انتظار برای پاسخ - بهتر است منتظر ظاهر شدن پاسخ جدید باشیم
                wait.Until(d => d.FindElements(By.CssSelector(".message-content")).Count > 0);
                await Task.Delay(3000); // 3 ثانیه اضافه برای اطمینان

                // یافتن آخرین پاسخ چت
                var chatResponses = _driver.FindElements(By.CssSelector(".message-content"));
                if (chatResponses.Count > 0)
                {
                    string translatedText = chatResponses.Last().Text;
                    return CleanTranslatedText(translatedText);
                }

                return text;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطا در ترجمه: {ex.Message}");
                return text;
            }
        }
        private string CleanTranslatedText(string text)
        {
            // حذف بخش‌های اضافی از پاسخ
            if (text.Contains("ترجمه فارسی:"))
                text = text.Split(new[] { "ترجمه فارسی:" }, StringSplitOptions.RemoveEmptyEntries)[1].Trim();

            return text;
        }

        private string ConvertVttToSrt(string vttContent)
        {
            var sb = new StringBuilder();
            var blocks = vttContent.Split(new[] { "\n\n" }, StringSplitOptions.RemoveEmptyEntries);
            int index = 1;

            foreach (var block in blocks)
            {
                var lines = block.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                if (lines.Length < 2)
                    continue;

                // شماره زیرنویس
                sb.AppendLine(index.ToString());
                index++;

                // زمان‌بندی
                string timing = lines[0].Trim();
                if (timing.Contains(" --> "))
                {
                    timing = timing.Replace(".", ",");
                    sb.AppendLine(timing);
                }

                // متن زیرنویس
                for (int i = 1; i < lines.Length; i++)
                {
                    sb.AppendLine(lines[i].Trim());
                }

                sb.AppendLine();
            }

            return sb.ToString().Trim();
        }
        private void ConvertVttToSrt(string inputPath, string outputPath)
        {
            string[] lines = File.ReadAllLines(inputPath, Encoding.UTF8);
            StringBuilder srtContent = new StringBuilder();
            int subtitleNumber = 1;

            // Skip the WEBVTT header and optional metadata
            int startLine = 0;
            while (startLine < lines.Length &&
                  (string.IsNullOrWhiteSpace(lines[startLine]) ||
                   lines[startLine].StartsWith("WEBVTT") ||
                   lines[startLine].StartsWith("NOTE") ||
                   lines[startLine].StartsWith("REGION")))
            {
                startLine++;
            }

            for (int i = startLine; i < lines.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(lines[i]))
                    continue;

                if (lines[i].Contains("-->"))
                {
                    srtContent.AppendLine(subtitleNumber.ToString());
                    subtitleNumber++;

                    string timestampLine = ConvertVttTimeToSrt(lines[i]);
                    srtContent.AppendLine(timestampLine);

                    i++;
                    while (i < lines.Length && !string.IsNullOrWhiteSpace(lines[i]) && !lines[i].Contains("-->"))
                    {
                        srtContent.AppendLine(lines[i]);
                        i++;
                    }

                    srtContent.AppendLine();
                }
            }

            File.WriteAllText(outputPath, srtContent.ToString(), Encoding.UTF8);
        }

        private string ConvertVttTimeToSrt(string vttTime)
        {
            string cleanedTime = Regex.Replace(vttTime, @"\s*-->\s*", " --> ");
            string[] parts = cleanedTime.Split(new[] { " --> " }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length != 2)
                return vttTime;

            string startTime = FormatTime(parts[0]);
            string endTime = FormatTime(parts[1]);

            return $"{startTime} --> {endTime}";
        }

        private string FormatTime(string time)
        {
            time = time.Trim().Replace(".", ",");

            // Handle cases where milliseconds might be missing
            if (!time.Contains(","))
            {
                time += ",000";
            }

            string[] parts = time.Split(':');

            // SS,mmm format
            if (parts.Length == 1)
            {
                string[] secParts = parts[0].Split(',');
                return $"00:00:{secParts[0].PadLeft(2, '0')},{secParts[1].PadRight(3, '0').Substring(0, 3)}";
            }

            // MM:SS,mmm format
            if (parts.Length == 2)
            {
                string[] secParts = parts[1].Split(',');
                return $"00:{parts[0].PadLeft(2, '0')}:{secParts[0].PadLeft(2, '0')},{secParts[1].PadRight(3, '0').Substring(0, 3)}";
            }

            // HH:MM:SS,mmm format
            if (parts.Length == 3)
            {
                string[] secParts = parts[2].Split(',');
                return $"{parts[0].PadLeft(2, '0')}:{parts[1].PadLeft(2, '0')}:{secParts[0].PadLeft(2, '0')},{secParts[1].PadRight(3, '0').Substring(0, 3)}";
            }

            return time;
        }
    }
}
