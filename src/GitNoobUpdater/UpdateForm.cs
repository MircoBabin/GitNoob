using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace GitNoobUpdater
{
    public partial class UpdateForm : Form
    {
        private Utils Utils;
        private DownloadUrl download;
        private volatile bool cancelUpdate;
        private UpdateStatus updateStatus;

        private enum UpdateStatus { 
            Initializing, 
            Downloading, 
            WaitingFoGitNoobToStop, 
            DeletingCurrentGitNoob,
            UnpackingNewGitNoob,
            Done,

            Error,
            Canceled,
        };

        public UpdateForm(Utils Utils, DownloadUrl download)
        {
            this.Utils = Utils;
            this.download = download;

            InitializeComponent();

            cancelUpdate = false;
            btnAction.Click += BtnAction_Click;
            this.FormClosing += UpdateForm_FormClosing;
            updateSetForm(UpdateStatus.Initializing, "Initializing...", "Cancel");

            new Thread(update).Start();
        }

        private void UpdateForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
        }

        private void BtnAction_Click(object sender, EventArgs e)
        {
            cancelUpdate = true;

            switch(updateStatus)
            {
                case UpdateStatus.Canceled:
                case UpdateStatus.Error:
                case UpdateStatus.Done:
                    Program.Exit();
                    break;
            }
        }

        private void update()
        {
            if (updateIsCanceled()) return;

            updateSetForm(UpdateStatus.Downloading, "Downloading...", "Cancel");

            string zipFilename;
            try
            {
                zipFilename = Utils.downloadVersion(download);
            }
            catch (Exception ex)
            {
                updateError("Downloading failed: " + ex.Message);
                return;
            }

            updateSetForm(UpdateStatus.WaitingFoGitNoobToStop, "Waiting for GitNoob to stop...", "Cancel");
            try
            {
                while (true)
                {
                    if (updateIsCanceled()) return;

                    if (!Utils.isGitNoobRunning()) break;

                    updateSetForm(UpdateStatus.WaitingFoGitNoobToStop, "Please close GitNoob.", "Cancel");
                    Thread.Sleep(1000);
                }
            }
            catch (Exception ex)
            {
                updateError("Waiting for GitNoob to stop failed: " + ex.Message);
                return;
            }

            updateSetForm(UpdateStatus.DeletingCurrentGitNoob, "Removing current GitNoob...", null);
            List<string> currentFiles;
            try
            {
                currentFiles = Utils.InstallationFilenames();
            }
            catch (Exception ex)
            {
                updateError("Retrieving current installation filenames failed: " + ex.Message);
                return;
            }

            if (currentFiles != null)
            {
                foreach (var filename in currentFiles)
                {
                    try
                    {
                        Utils.DeleteFileRetried(filename);
                    }
                    catch (Exception ex)
                    {
                        updateError("Removing current GitNoob failed: " + ex.Message);
                        return;
                    }
                }
            }

            updateSetForm(UpdateStatus.UnpackingNewGitNoob, "Installing new GitNoob...", null);
            try
            {
                // NuGet package: DotNetZip 1.16.0
                // https://www.nuget.org/packages/DotNetZip/


                using (var zip = new Ionic.Zip.ZipFile(zipFilename))
                {
                    zip.ExtractAll(Utils.GitNoobPath, Ionic.Zip.ExtractExistingFileAction.OverwriteSilently);
                }
            }
            catch (Exception ex)
            {
                updateError("Installing new GitNoob failed: " + ex.Message);
                return;
            }

            updateSetForm(UpdateStatus.Done, "Installation of version " + download.version.FullVersion + " finished", "Exit");
        }

        private bool updateIsCanceled()
        {
            if (!cancelUpdate) return false;

            updateStatus = UpdateStatus.Canceled;

            Program.Exit();
            return true;
        }

        private void updateError(string text)
        {
            updateSetForm(UpdateStatus.Error, text, "Error");
        }

        private void updateSetForm(UpdateStatus status, string progress, string buttonText)
        {
            var execute = (MethodInvoker)delegate
            {
                updateStatus = status;

                lblProgress.Text = progress;

                if (!string.IsNullOrWhiteSpace(buttonText))
                {
                    btnAction.Text = buttonText;
                    btnAction.Visible = true;
                }
                else
                {
                    btnAction.Text = string.Empty;
                    btnAction.Visible = false;
                }
            };

            if (this.InvokeRequired)
            {
                this.Invoke(execute);
            }
            else
            {
                execute();
            }
        }
    }
}
