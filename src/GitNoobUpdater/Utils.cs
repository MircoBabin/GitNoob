using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;

namespace GitNoobUpdater
{
    public class Utils
    {
        public string GitNoobPath { get; private set; }
        public string urlForGetLatestDownloadUrl { get; private set; }

        public Utils(string GitNoobPath, string urlForGetLatestDownloadUrl)
        {
            this.GitNoobPath = GitNoobPath;
            this.urlForGetLatestDownloadUrl = urlForGetLatestDownloadUrl;
        }

        private string GitNoobExe()
        {
            string exe = Path.Combine(GitNoobPath, "GitNoob.exe");
            if (!File.Exists(exe)) throw new Exception("File does not exist: " + exe);

            return exe;
        }

        private void RunGitNoobExe(string commandline)
        {
            var info = new ProcessStartInfo
            {
                FileName = GitNoobExe(),
                Arguments = commandline,

                UseShellExecute = false,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden,
            };

            var p = Process.Start(info);
            p.WaitForExit();
        }

        public bool isGitNoobRunning()
        {
            try
            {
                using (FileStream fs = File.Open(GitNoobExe(), FileMode.Open, FileAccess.ReadWrite, FileShare.None)) { }
                return false;
            } 
            catch
            {
                return true;
            }
        }

        public GitNoobVersion Version()
        {
            GitNoobVersion result;

            string filename = Path.GetTempFileName();
            try
            {
                RunGitNoobExe("\"--version=" + filename + "\"");

                result = new GitNoobVersion(File.ReadAllText(filename, System.Text.Encoding.ASCII).Trim());
            }
            finally
            {
                try { File.Delete(filename); } catch { }
            }

            return result;
        }

        public List<string> InstallationFilenames()
        {
            List<string> filenames = new List<string>();

            //GitNoobUpdater.exe
            filenames.AddRange(Program.installationFilenames);

            //GitNoob.exe
            string outputFilename = Path.GetTempFileName();
            try
            {
                RunGitNoobExe("\"--installationFilenames=" + outputFilename + "\"");

                foreach(var line in File.ReadAllText(outputFilename, System.Text.Encoding.ASCII).Split('\n'))
                {
                    var filename = line.Trim();
                    if (!string.IsNullOrWhiteSpace(filename)) filenames.Add(filename);
                }
            }
            finally
            {
                try { File.Delete(outputFilename); } catch { }
            }

            return filenames;
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

        public DownloadUrl getLatestDownloadUrl()
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

            return new DownloadUrl(url, new GitNoobVersion(filename.Substring(prefix.Length, filename.Length - prefix.Length - suffix.Length)));
        }

        public string downloadVersion(DownloadUrl url)
        {
            string filename = Path.Combine(GitNoobPath, "GitNoob.zip");
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
                using (var response = httpGet(url.downloadUrl))
                {
                    using (Stream input = response.GetResponseStream())
                    {
                        input.CopyTo(output);
                    }
                }
            }

            return filename;
        }

        public void DeleteFileRetried(string filename, int timeoutSeconds = 30)
        {
            var StartTime = DateTime.Now;

            while (true)
            {
                try
                {
                    if (!File.Exists(filename)) break;

                    File.Delete(filename);
                }
                catch (Exception ex)
                {
                    if (DateTime.Now.Subtract(StartTime).Seconds > timeoutSeconds)
                        throw new Exception("Deleting " + filename + " failed.\n\n" + ex.Message);
                }
            }
        }
    }
}
