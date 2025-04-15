namespace VTTConverter
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            txtVttFile = new TextBox();
            btnBrowse = new Button();
            btnTranslate = new Button();
            label1 = new Label();
            progressBar1 = new ProgressBar();
            lblStatus = new Label();
            SuspendLayout();
            // 
            // txtVttFile
            // 
            txtVttFile.Location = new Point(10, 36);
            txtVttFile.Name = "txtVttFile";
            txtVttFile.ReadOnly = true;
            txtVttFile.Size = new Size(307, 23);
            txtVttFile.TabIndex = 0;
            // 
            // btnBrowse
            // 
            btnBrowse.Location = new Point(322, 36);
            btnBrowse.Name = "btnBrowse";
            btnBrowse.Size = new Size(66, 22);
            btnBrowse.TabIndex = 1;
            btnBrowse.Text = "انتخاب فایل";
            btnBrowse.UseVisualStyleBackColor = true;
            btnBrowse.Click += btnBrowse_Click;
            // 
            // btnTranslate
            // 
            btnTranslate.Location = new Point(10, 77);
            btnTranslate.Name = "btnTranslate";
            btnTranslate.Size = new Size(377, 38);
            btnTranslate.TabIndex = 2;
            btnTranslate.Text = "تبدیل به SRT";
            btnTranslate.UseVisualStyleBackColor = true;
            btnTranslate.Click += btnTranslate_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(10, 17);
            label1.Name = "label1";
            label1.Size = new Size(87, 15);
            label1.TabIndex = 3;
            label1.Text = "فایل VTT ورودی:";
            // 
            // progressBar1
            // 
            progressBar1.Location = new Point(10, 130);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new Size(377, 22);
            progressBar1.Style = ProgressBarStyle.Marquee;
            progressBar1.TabIndex = 4;
            progressBar1.Visible = false;
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Location = new Point(10, 161);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(0, 15);
            lblStatus.TabIndex = 5;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(396, 171);
            Controls.Add(lblStatus);
            Controls.Add(progressBar1);
            Controls.Add(label1);
            Controls.Add(btnTranslate);
            Controls.Add(btnBrowse);
            Controls.Add(txtVttFile);
            Name = "Form1";
            RightToLeft = RightToLeft.Yes;
            RightToLeftLayout = true;
            Text = "ترجمه و تبدیل VTT به SRT";
            ResumeLayout(false);
            PerformLayout();
        }
        private System.Windows.Forms.TextBox txtVttFile;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.Button btnTranslate;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label lblStatus;
        #endregion

        private Label label2;
        private Button button1;
        private TextBox textBox1;
    }
}
