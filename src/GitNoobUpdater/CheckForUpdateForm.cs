using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace GitNoobUpdater
{
    public partial class CheckForUpdateForm : Form
    {
        private Utils Utils;
        private bool forcedUpdate;
        private DownloadUrl latestDownloadUrl;
        private GitNoobVersion currentVersion;

        public CheckForUpdateForm(Utils Utils, bool forcedUpdate)
        {
            this.Utils = Utils;
            this.forcedUpdate = forcedUpdate;

            InitializeComponent();

            btnUpdate.Visible = false;
            btnNoNewVersionAvailable.Visible = false;
            btnManualCheck.Visible = false;

            btnUpdate.Click += BtnUpdate_Click;
            btnNoNewVersionAvailable.Click += BtnNoNewVersionAvailable_Click;
            btnManualCheck.Click += BtnManualCheck_Click;

            new Thread(BuildForm).Start();
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            btnUpdate.Visible = false;
            Program.RestartAsAdministrator(Utils.GitNoobPath, latestDownloadUrl);
            Program.Exit();
        }

        private void BtnNoNewVersionAvailable_Click(object sender, EventArgs e)
        {
            Program.Exit();
        }

        private void BtnManualCheck_Click(object sender, EventArgs e)
        {
            var info = new ProcessStartInfo
            {
                FileName = Program.urlForShowLatest,

                UseShellExecute = true,
            };

            Process.Start(info);
            Program.Exit();
        }

        private void BuildForm()
        {
            currentVersion = Utils.Version();
            this.Invoke((MethodInvoker)delegate
            {
                lblCurrentVersionValue.Text = currentVersion.ToString();
            });

            latestDownloadUrl = null;
            try
            {
                latestDownloadUrl = Utils.getLatestDownloadUrl();
            }
            catch { }

            this.Invoke((MethodInvoker)delegate
            {
                if (latestDownloadUrl != null)
                {
                    lblLatestVersionValue.Text = latestDownloadUrl.version.ToString();

                    if (forcedUpdate || latestDownloadUrl.version.isGreaterThan(currentVersion))
                    {
                        btnUpdate.Visible = true;
                    }
                    else
                    {
                        btnNoNewVersionAvailable.Visible = true;
                    }
                }
                else
                {
                    lblLatestVersionValue.Text = "Error retrieving";
                    btnManualCheck.Visible = true;
                }
            });
        }
    }
}
