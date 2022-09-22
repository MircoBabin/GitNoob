using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows.Forms;

namespace GitNoobUpdater
{
    public partial class UpdateForm : Form
    {
        private const string urlForShowLatest = "https://github.com/MircoBabin/GitNoob/releases/latest";
        private const string urlForGetLatestDownloadUrl = "https://github.com/MircoBabin/GitNoob/releases/latest/download/release.download.zip.url-location";

        private string latestDownloadUrl;
        private GitNoobVersion latestVersion;
        private GitNoobVersion currentVersion;


        public UpdateForm()
        {
            /*
             * todo: Work In Progress
             * GitNoobUpdater is not working yet
             * 
             */

            InitializeComponent();

            btnUpdate.Visible = false;
            btnNoNewVersionAvailable.Visible = false;
            btnManualCheck.Visible = false;

            btnUpdate.Click += BtnUpdate_Click;
            btnNoNewVersionAvailable.Click += BtnNoNewVersionAvailable_Click;
            btnManualCheck.Click += BtnManualCheck_Click;

            new Thread(BuildForm).Start();
        }

        private void BtnUpdate_Click(object sender, System.EventArgs e)
        {
            btnUpdate.Visible = false;
            new Thread(update).Start();
        }

        private void BtnNoNewVersionAvailable_Click(object sender, System.EventArgs e)
        {
            Exit();
        }

        private void BtnManualCheck_Click(object sender, System.EventArgs e)
        {
            var info = new ProcessStartInfo
            {
                FileName = urlForShowLatest,

                UseShellExecute = true,
            };

            Process.Start(info);
            Exit();
        }

        private void Exit()
        {
            Application.Exit();
        }

        private void BuildForm()
        {
            currentVersion = GitNoobQuery.Version();
            this.Invoke((MethodInvoker)delegate
            {
                lblCurrentVersionValue.Text = currentVersion.ToString();
            });

            latestDownloadUrl = string.Empty;
            latestVersion = null;
            try
            {
                getLatestDownloadUrl();
            }
            catch { }

            this.Invoke((MethodInvoker)delegate
            {
                if (latestVersion != null)
                {
                    lblLatestVersionValue.Text = latestVersion.ToString();

                    if (latestVersion.isGreaterThan(currentVersion))
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

        private void update()
        {
            try
            {
                downloadLatest();
            }
            catch { }
        }

        private HttpWebResponse httpGet(string url)
        {
            var systemproxy = WebRequest.GetSystemWebProxy();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls |
                SecurityProtocolType.Ssl3 |
                (SecurityProtocolType)768 /* TLS 1.1 */ |
                (SecurityProtocolType)3072 /* TLS 1.2 */;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.Proxy = systemproxy;
            request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
            request.AllowAutoRedirect = true;

            return (HttpWebResponse)request.GetResponse();
        }

        private void getLatestDownloadUrl()
        {
            string url;
            using (var response = httpGet(urlForGetLatestDownloadUrl))
            {
                using (Stream stream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        url = reader.ReadToEnd();
                        // https://github.com/MircoBabin/GitNoob/releases/download/1.15/GitNoob-1.15.zip
                    }
                }
            }

            var p = url.LastIndexOf('/');
            if (p < 0) throw new Exception("Latest download url is invalid: " + url);
            var filename = url.Substring(p + 1);
            const string prefix = "GitNoob-";
            const string suffix = ".zip";
            if (!filename.StartsWith(prefix)) throw new Exception("Latest download url is invalid: " + url);
            if (!filename.EndsWith(suffix)) throw new Exception("Latest download url is invalid: " + url);

            latestVersion = new GitNoobVersion(filename.Substring(prefix.Length, filename.Length - prefix.Length - suffix.Length));
            latestDownloadUrl = url;
        }

        private void downloadLatest()
        {
            Process me = Process.GetCurrentProcess();
            string executableFileName = me.Modules[0].FileName;
            string programPath = Path.GetFullPath(Path.GetDirectoryName(executableFileName));

            string filename = Path.Combine(programPath, "GitNoob.zip");
            if (File.Exists(filename))
            {
                File.Delete(filename);
                if (File.Exists(filename))
                {
                    throw new Exception("Could not delete: " + filename);
                }
            }

            using (Stream output = File.OpenWrite(filename))
            {
                using (var response = httpGet(latestDownloadUrl))
                {
                    using (Stream input = response.GetResponseStream())
                    {
                        input.CopyTo(output);
                    }
                }
            }
        }
    }
}
