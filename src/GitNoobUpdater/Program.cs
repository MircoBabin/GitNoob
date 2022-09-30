using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace GitNoobUpdater
{
    static class Program
    {
        public const string urlForShowLatest = "https://github.com/MircoBabin/GitNoob/releases/latest";
        private const string urlForGetLatestDownloadUrl = "https://github.com/MircoBabin/GitNoob/releases/latest/download/release.download.zip.url-location";

        private static string programPath;
        private static string GitNoobPath;

        public static List<string> installationFilenames = new List<string>()
        {
                "GitNoobUpdater.exe",
                "GitNoobUpdater.exe.config",
                "DotNetZip.dll",
        };

        private static void OutputVersion(string outputFilename)
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;

            File.WriteAllText(outputFilename, version.Major + "." + version.Minor, Encoding.ASCII);
        }

        private static void OutputInstallationFilenames(string outputFilename)
        {
            StringBuilder output = new StringBuilder();
            foreach (var filename in installationFilenames)
            {
                output.AppendLine(filename);
            }

            File.WriteAllText(outputFilename, output.ToString(), Encoding.ASCII);
        }

        public static void RestartAsAdministrator(string GitNoobPath, DownloadUrl download)
        {
            string args = new ProgramArguments(GitNoobPath, download).Serialize();

            // copy to tempdirectory
            string tempFolder = Path.Combine(Path.GetTempPath(), "GitNoobUpdater");
            if (Directory.Exists(tempFolder)) Directory.Delete(tempFolder, true);
            Directory.CreateDirectory(tempFolder);

            foreach(var filename in installationFilenames)
            {
                var srcFilename = Path.Combine(programPath, filename);
                var dstFilename = Path.Combine(tempFolder, filename);

                File.Copy(srcFilename, dstFilename);
            }

            // restart as administrator
            var info = new ProcessStartInfo
            {
                FileName = Path.Combine(tempFolder, "GitNoobUpdater.exe"),
                Arguments = "\"update\" \"" + args + "\"",

                UseShellExecute = true,
                Verb = "runas",
            };

            Process.Start(info);
        }

        public static void Exit()
        {
            Environment.Exit(0);
        }

        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length == 1 &&
                args[0].Length > 10 && args[0].Substring(0, 10).ToLowerInvariant() == "--version=")
            {
                // "--version=d:\Projects\GitNoob\assets\Release_version.txt"
                var filename = args[0].Substring(10).Trim();
                if (!string.IsNullOrWhiteSpace(filename) && filename.IndexOfAny(Path.GetInvalidPathChars()) < 0)
                {
                    OutputVersion(filename);
                }
                return;
            }

            if (args.Length == 1 &&
                args[0].Length > 24 && args[0].Substring(0, 24).ToLowerInvariant() == "--installationfilenames=")
            {
                // "--installationFilenames=d:\Projects\GitNoob\assets\Release_filenames.txt"
                var filename = args[0].Substring(24).Trim();
                if (!string.IsNullOrWhiteSpace(filename) && filename.IndexOfAny(Path.GetInvalidPathChars()) < 0)
                {
                    OutputInstallationFilenames(filename);
                }
                return;
            }

            if (args.Length == 1 &&
                args[0].ToLowerInvariant() == "debugger")
            {
                // "debugger"

                Debugger.Launch();
            }

            Process me = Process.GetCurrentProcess();
            programPath = Path.GetFullPath(Path.GetDirectoryName(me.Modules[0].FileName));
            GitNoobPath = programPath;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (args.Length == 2 &&
                args[0] == "update")
            {
                ProgramArguments pargs = null;
                try
                {
                    pargs = ProgramArguments.Deserialize(args[1]);
                }
                catch { }

                if (pargs != null)
                {
                    GitNoobPath = pargs.GitNoobPath;
                    Application.Run(new UpdateForm(new Utils(GitNoobPath, urlForGetLatestDownloadUrl), pargs.download));
                    return;
                }
            }

            bool forcedUpdate = Debugger.IsAttached;
            Application.Run(new CheckForUpdateForm(new Utils(GitNoobPath, urlForGetLatestDownloadUrl), forcedUpdate));
        }
    }
}
