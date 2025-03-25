namespace BasicAutoPatch
{
    partial class Main
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            btnStart = new Button();
            progressBar1 = new ProgressBar();
            lblStatus = new Label();
            lblFile = new Label();
            lblDownloadSpeed = new Label();
            lblSpeed = new Label();
            SuspendLayout();
            // 
            // btnStart
            // 
            btnStart.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnStart.Location = new Point(169, 125);
            btnStart.Name = "btnStart";
            btnStart.Size = new Size(253, 48);
            btnStart.TabIndex = 0;
            btnStart.Text = "Download";
            btnStart.UseVisualStyleBackColor = true;
            btnStart.Click += btnStart_Click;
            // 
            // progressBar1
            // 
            progressBar1.Location = new Point(27, 29);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new Size(578, 23);
            progressBar1.TabIndex = 1;
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Location = new Point(27, 66);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(45, 15);
            lblStatus.TabIndex = 2;
            lblStatus.Text = "Status :";
            // 
            // lblFile
            // 
            lblFile.AutoSize = true;
            lblFile.Location = new Point(27, 96);
            lblFile.Name = "lblFile";
            lblFile.Size = new Size(31, 15);
            lblFile.TabIndex = 3;
            lblFile.Text = "File :";
            // 
            // lblDownloadSpeed
            // 
            lblDownloadSpeed.AutoSize = true;
            lblDownloadSpeed.Location = new Point(412, 66);
            lblDownloadSpeed.Name = "lblDownloadSpeed";
            lblDownloadSpeed.Size = new Size(80, 15);
            lblDownloadSpeed.TabIndex = 4;
            lblDownloadSpeed.Text = "Downloaded :";
            // 
            // lblSpeed
            // 
            lblSpeed.AutoSize = true;
            lblSpeed.Location = new Point(412, 96);
            lblSpeed.Name = "lblSpeed";
            lblSpeed.Size = new Size(102, 15);
            lblSpeed.TabIndex = 5;
            lblSpeed.Text = "Download Speed :";
            // 
            // Main
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(626, 185);
            Controls.Add(lblSpeed);
            Controls.Add(lblDownloadSpeed);
            Controls.Add(lblFile);
            Controls.Add(lblStatus);
            Controls.Add(progressBar1);
            Controls.Add(btnStart);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Main";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Main";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnStart;
        private ProgressBar progressBar1;
        private Label lblStatus;
        private Label lblFile;
        private Label lblDownloadSpeed;
        private Label lblSpeed;
    }
}