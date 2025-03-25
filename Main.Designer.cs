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
            radDx8 = new RadioButton();
            radDx9 = new RadioButton();
            checkNoEffects = new CheckBox();
            SuspendLayout();
            // 
            // btnStart
            // 
            btnStart.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnStart.Location = new Point(12, 40);
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
            // radDx8
            // 
            radDx8.AutoSize = true;
            radDx8.Checked = true;
            radDx8.Location = new Point(13, 11);
            radDx8.Name = "radDx8";
            radDx8.Size = new Size(48, 19);
            radDx8.TabIndex = 6;
            radDx8.TabStop = true;
            radDx8.Text = "dx_8";
            radDx8.UseVisualStyleBackColor = true;
            radDx8.Visible = false;
            // 
            // radDx9
            // 
            radDx9.AutoSize = true;
            radDx9.Location = new Point(67, 11);
            radDx9.Name = "radDx9";
            radDx9.Size = new Size(48, 19);
            radDx9.TabIndex = 7;
            radDx9.Text = "dx_9";
            radDx9.UseVisualStyleBackColor = true;
            radDx9.Visible = false;
            // 
            // checkNoEffects
            // 
            checkNoEffects.AutoSize = true;
            checkNoEffects.Location = new Point(121, 12);
            checkNoEffects.Name = "checkNoEffects";
            checkNoEffects.Size = new Size(89, 19);
            checkNoEffects.TabIndex = 8;
            checkNoEffects.Text = "Hide Effects";
            checkNoEffects.UseVisualStyleBackColor = true;
            checkNoEffects.Visible = false;
            checkNoEffects.CheckedChanged += checkNoEffects_CheckedChanged;
            // 
            // Main
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(281, 100);
            Controls.Add(checkNoEffects);
            Controls.Add(radDx9);
            Controls.Add(radDx8);
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
        private RadioButton radDx8;
        private RadioButton radDx9;
        private CheckBox checkNoEffects;
    }
}