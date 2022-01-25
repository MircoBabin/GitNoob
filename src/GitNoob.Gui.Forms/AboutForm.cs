using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace GitNoob.Gui.Forms
{
    public partial class AboutForm : Form
    {
        private string _licenseText;
        public AboutForm(string licenseText)
        {
            _licenseText = licenseText;

            InitializeComponent();
            BuildForm();

            this.Resize += AboutForm_Resize;
        }

        private void BuildForm()
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            this.NameLabel.Text += " " + version.Major + "." + version.Minor;

            string url = "https://github.com/MircoBabin/GitNoob/";
            LinkLabelUtils.SetLinkLabel(this.WebsiteLinkLabel, "[link website]" + url + "[/link]", new Dictionary<string, System.Action>
            {
                { "website", () => { System.Diagnostics.Process.Start(url); } },
            });
            WebsiteLinkLabel.LinkClicked += WebsiteLinkLabel_LinkClicked;

            LicenseLabel.Text = _licenseText;
            LicensePanel.Size = new System.Drawing.Size(this.ClientRectangle.Width - LicensePanel.Left - LicensePanel.Left, this.ClientRectangle.Height - LicensePanel.Top - 10);
        }

        private void AboutForm_Resize(object sender, System.EventArgs e)
        {
            LicensePanel.Size = new System.Drawing.Size(this.ClientRectangle.Width - LicensePanel.Left - LicensePanel.Left, this.ClientRectangle.Height - LicensePanel.Top - 10);
        }

        private void WebsiteLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LinkLabelUtils.ExecuteLinkClicked(e);
        }
    }
}