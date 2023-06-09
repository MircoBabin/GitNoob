using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace GitNoob
{
    static class Program
    {
        private static class NativeMethods
        {
            [DllImport("user32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool SetForegroundWindow(IntPtr hWnd);
        }

        public static List<string> installationFilenames = new List<string>()
        {
                "GitNoob.exe",
                "GitNoob.exe.config",

                "GitNoob.Config.dll",

                "GitNoob.Git.dll",

                "GitNoob.Git.Result.dll",

                "GitNoob.Gui.Forms.dll",
                "GitNoob.Gui.Forms.dll.config",

                "GitNoob.Gui.Program.dll",

                "GitNoob.Gui.Visualizer.dll",

                "GitNoob.ProjectTypes.dll",

                "GitNoob.Utils.dll",
        };

        private static void OutputVersion(string outputFilename)
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;

            File.WriteAllText(outputFilename, version.Major + "." + version.Minor, Encoding.ASCII);
        }

        private static void OutputInstallationFilenames(string outputFilename)
        {
            StringBuilder output = new StringBuilder();
            foreach(var filename in installationFilenames)
            {
                output.AppendLine(filename);
            }

            File.WriteAllText(outputFilename, output.ToString(), Encoding.ASCII);
        }

        private static bool handleVersionCommand(string[] args)
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
                return true;
            }

            return false;
        }

        private static bool handleInstallationFilesCommand(string[] args)
        {
            if (args.Length == 1 &&
                args[0].Length > 24 && args[0].Substring(0, 24).ToLowerInvariant() == "--installationfilenames=")
            {
                // "--installationFilenames=d:\Projects\GitNoob\assets\Release_filenames.txt"
                var filename = args[0].Substring(24).Trim();
                if (!string.IsNullOrWhiteSpace(filename) && filename.IndexOfAny(Path.GetInvalidPathChars()) < 0)
                {
                    OutputInstallationFilenames(filename);
                }
                return true;
            }

            return false;
        }

        private static bool handleBlocked(Bootstrapper bootstrapper)
        {
            var blocked = Utils.ZoneIdentifier.IsFileBlocked.CheckFile(bootstrapper.executableFileName);
            switch (blocked.Status)
            {
                case Utils.ZoneIdentifier.IsFileBlocked.FileBlockedStatus.No:
                    break;

                case Utils.ZoneIdentifier.IsFileBlocked.FileBlockedStatus.Yes:
                    MessageBox.Show("The executable \"" + bootstrapper.executableFileName + "\" is blocked. Use Windows Explorer and via properties unblock all GitNoob files.", "GitNoob");
                    return true;

                default:
                    {
                        var result = MessageBox.Show("The blocked status of the executable \"" + bootstrapper.executableFileName + "\" could not be retrieved." + Environment.NewLine + blocked.Exception.Message + Environment.NewLine + Environment.NewLine + "Continue ?", "GitNoob", MessageBoxButtons.YesNo);
                        if (result != DialogResult.Yes) return true;
                    }
                    break;
            }

            return false;
        }

        private static bool handleRootConfigurationFilename(string[] args, Bootstrapper bootstrapper)
        {
            if (args.Length >= 1)
            {
                bootstrapper.rootConfigurationFilename = Path.GetFullPath(args[0]);
            }

            if (!File.Exists(bootstrapper.rootConfigurationFilename))
            {
                MessageBox.Show("Root configuration file does not exist:" + Environment.NewLine + bootstrapper.rootConfigurationFilename, "GitNoob");
                return true;
            }

            return false;
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            var bootstrapper = new Bootstrapper();

            if (handleVersionCommand(args)) return;
            if (handleInstallationFilesCommand(args)) return;
            if (handleBlocked(bootstrapper)) return;
            if (handleRootConfigurationFilename(args, bootstrapper)) return;

            string singleInstanceMutexName;
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] hash = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(bootstrapper.rootConfigurationFilename));

                singleInstanceMutexName = "GitNoob." + BitConverter.ToString(hash).Replace("-", string.Empty);
            }

            bool firstInstance = false;
            using (Mutex singleInstanceMutex = new Mutex(true, singleInstanceMutexName, out firstInstance))
            {
                if (firstInstance)
                {
                    try
                    {
                        bootstrapper.Run();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "GitNoob Startup Error");
                        return;
                    }
                }
                else
                {
                    try
                    {
                        foreach (Process proc in Process.GetProcessesByName(bootstrapper.me.ProcessName))
                        {
                            if (proc.Id != bootstrapper.me.Id && proc.Modules[0].FileName == bootstrapper.executableFileName)
                            {
                                NativeMethods.SetForegroundWindow(proc.MainWindowHandle);
                                break;
                            }
                        }
                    }
                    catch { }
                }
            }
        }
    }
}
