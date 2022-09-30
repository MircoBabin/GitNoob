namespace GitNoobUpdater
{
    partial class CheckForUpdateForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CheckForUpdateForm));
            this.lblCurrentVersion = new System.Windows.Forms.Label();
            this.lblLatestVersion = new System.Windows.Forms.Label();
            this.lblCurrentVersionValue = new System.Windows.Forms.Label();
            this.lblLatestVersionValue = new System.Windows.Forms.Label();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.btnNoNewVersionAvailable = new System.Windows.Forms.Button();
            this.btnManualCheck = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblCurrentVersion
            // 
            this.lblCurrentVersion.AutoSize = true;
            this.lblCurrentVersion.Location = new System.Drawing.Point(10, 10);
            this.lblCurrentVersion.Name = "lblCurrentVersion";
            this.lblCurrentVersion.Size = new System.Drawing.Size(107, 19);
            this.lblCurrentVersion.TabIndex = 0;
            this.lblCurrentVersion.Text = "Current version:";
            // 
            // lblLatestVersion
            // 
            this.lblLatestVersion.AutoSize = true;
            this.lblLatestVersion.Location = new System.Drawing.Point(10, 45);
            this.lblLatestVersion.Name = "lblLatestVersion";
            this.lblLatestVersion.Size = new System.Drawing.Size(97, 19);
            this.lblLatestVersion.TabIndex = 1;
            this.lblLatestVersion.Text = "Latest version:";
            // 
            // lblCurrentVersionValue
            // 
            this.lblCurrentVersionValue.AutoSize = true;
            this.lblCurrentVersionValue.Location = new System.Drawing.Point(124, 10);
            this.lblCurrentVersionValue.Name = "lblCurrentVersionValue";
            this.lblCurrentVersionValue.Size = new System.Drawing.Size(18, 19);
            this.lblCurrentVersionValue.TabIndex = 2;
            this.lblCurrentVersionValue.Text = "...";
            // 
            // lblLatestVersionValue
            // 
            this.lblLatestVersionValue.AutoSize = true;
            this.lblLatestVersionValue.Location = new System.Drawing.Point(124, 45);
            this.lblLatestVersionValue.Name = "lblLatestVersionValue";
            this.lblLatestVersionValue.Size = new System.Drawing.Size(18, 19);
            this.lblLatestVersionValue.TabIndex = 3;
            this.lblLatestVersionValue.Text = "...";
            // 
            // btnUpdate
            // 
            this.btnUpdate.Location = new System.Drawing.Point(13, 83);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(399, 24);
            this.btnUpdate.TabIndex = 4;
            this.btnUpdate.Text = "Update";
            this.btnUpdate.UseVisualStyleBackColor = true;
            // 
            // btnNoNewVersionAvailable
            // 
            this.btnNoNewVersionAvailable.Location = new System.Drawing.Point(13, 83);
            this.btnNoNewVersionAvailable.Name = "btnNoNewVersionAvailable";
            this.btnNoNewVersionAvailable.Size = new System.Drawing.Size(399, 24);
            this.btnNoNewVersionAvailable.TabIndex = 5;
            this.btnNoNewVersionAvailable.Text = "No new update is available";
            this.btnNoNewVersionAvailable.UseVisualStyleBackColor = true;
            // 
            // btnManualCheck
            // 
            this.btnManualCheck.Location = new System.Drawing.Point(13, 83);
            this.btnManualCheck.Name = "btnManualCheck";
            this.btnManualCheck.Size = new System.Drawing.Size(399, 24);
            this.btnManualCheck.TabIndex = 6;
            this.btnManualCheck.Text = "Latest version is unretrievable, manually check in browser";
            this.btnManualCheck.UseVisualStyleBackColor = true;
            // 
            // UpdateForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(423, 126);
            this.Controls.Add(this.lblLatestVersionValue);
            this.Controls.Add(this.lblCurrentVersionValue);
            this.Controls.Add(this.lblLatestVersion);
            this.Controls.Add(this.lblCurrentVersion);
            this.Controls.Add(this.btnNoNewVersionAvailable);
            this.Controls.Add(this.btnUpdate);
            this.Controls.Add(this.btnManualCheck);
            this.Font = new System.Drawing.Font("Segoe UI", 7.8F);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "UpdateForm";
            this.Text = "GitNoob updater";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblCurrentVersion;
        private System.Windows.Forms.Label lblLatestVersion;
        private System.Windows.Forms.Label lblCurrentVersionValue;
        private System.Windows.Forms.Label lblLatestVersionValue;
        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.Button btnNoNewVersionAvailable;
        private System.Windows.Forms.Button btnManualCheck;
    }
}

