namespace GitNoob.Gui.Forms
{
    partial class AboutForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutForm));
            this.NameLabel = new System.Windows.Forms.Label();
            this.WebsiteLinkLabel = new System.Windows.Forms.LinkLabel();
            this.LicensePanel = new System.Windows.Forms.Panel();
            this.LicenseLabel = new System.Windows.Forms.Label();
            this.LicensePanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // NameLabel
            // 
            this.NameLabel.AutoSize = true;
            this.NameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NameLabel.Location = new System.Drawing.Point(12, 9);
            this.NameLabel.Name = "NameLabel";
            this.NameLabel.Size = new System.Drawing.Size(187, 29);
            this.NameLabel.TabIndex = 0;
            this.NameLabel.Text = "GitNoob version";
            // 
            // WebsiteLinkLabel
            // 
            this.WebsiteLinkLabel.AutoSize = true;
            this.WebsiteLinkLabel.Location = new System.Drawing.Point(14, 38);
            this.WebsiteLinkLabel.Name = "WebsiteLinkLabel";
            this.WebsiteLinkLabel.Size = new System.Drawing.Size(72, 17);
            this.WebsiteLinkLabel.TabIndex = 2;
            this.WebsiteLinkLabel.TabStop = true;
            this.WebsiteLinkLabel.Text = "linkLabel1";
            // 
            // LicensePanel
            // 
            this.LicensePanel.AutoScroll = true;
            this.LicensePanel.Controls.Add(this.LicenseLabel);
            this.LicensePanel.Location = new System.Drawing.Point(17, 70);
            this.LicensePanel.Name = "LicensePanel";
            this.LicensePanel.Size = new System.Drawing.Size(573, 209);
            this.LicensePanel.TabIndex = 3;
            // 
            // LicenseLabel
            // 
            this.LicenseLabel.AutoSize = true;
            this.LicenseLabel.Location = new System.Drawing.Point(5, 5);
            this.LicenseLabel.Name = "LicenseLabel";
            this.LicenseLabel.Size = new System.Drawing.Size(52, 17);
            this.LicenseLabel.TabIndex = 0;
            this.LicenseLabel.Text = "license";
            // 
            // AboutForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(594, 291);
            this.Controls.Add(this.LicensePanel);
            this.Controls.Add(this.WebsiteLinkLabel);
            this.Controls.Add(this.NameLabel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "AboutForm";
            this.Text = "About GitNoob";
            this.LicensePanel.ResumeLayout(false);
            this.LicensePanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label NameLabel;
        private System.Windows.Forms.LinkLabel WebsiteLinkLabel;
        private System.Windows.Forms.Panel LicensePanel;
        private System.Windows.Forms.Label LicenseLabel;
    }
}