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
                "GitNoob.Config.dll",
                "GitNoob.exe",
                "GitNoob.exe.config",
                "GitNoob.Git.dll",
                "GitNoob.Gui.Forms.dll",
                "GitNoob.Gui.Forms.dll.config",
                "GitNoob.Gui.Program.dll",
                "GitNoob.ProjectTypes.dll",
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

        private static bool CheckConfigs(List<Config.IConfig> configs)
        {
            Dictionary<string, string> workingDirectoryPaths = new Dictionary<string, string>();
            foreach (var config in configs)
            {
                foreach (var project in config.GetProjects())
                {
                    foreach (var item in project.WorkingDirectories)
                    {
                        var wd = item.Value;
                        var path = wd.Path.ToString().ToLowerInvariant();

                        var name = "Project \"" + project.Name + "\", working directory \"" + wd.Name + "\".";

                        if (workingDirectoryPaths.ContainsKey(path))
                        {
                            string message = "The directory \"" + path + "\" is used in multiple project working directories. This is not allowed." + Environment.NewLine +
                                Environment.NewLine +
                                workingDirectoryPaths[path] + Environment.NewLine +
                                name + Environment.NewLine +
                                Environment.NewLine +
                                "When having 2 mainbranches e.g. \"development\" and \"release\", create 2 directories MyProject-Development and MyProject-Release with their own main branch. The project is then stored 2 times on disk in 2 different directories." + Environment.NewLine;

                            MessageBox.Show(message, "GitNoob configuration error");
                            return false;
                        }

                        workingDirectoryPaths.Add(path, name);
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
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


            Process me = Process.GetCurrentProcess();
            string executableFileName = me.Modules[0].FileName;
            string programPath = Path.GetFullPath(Path.GetDirectoryName(executableFileName));

            {
                var blocked = ZoneIdentifier.IsFileBlocked.CheckFile(executableFileName);
                switch (blocked.Status)
                {
                    case ZoneIdentifier.IsFileBlocked.FileBlockedStatus.No:
                        break;

                    case ZoneIdentifier.IsFileBlocked.FileBlockedStatus.Yes:
                        MessageBox.Show("The executable \"" + executableFileName + "\" is blocked. Use Windows Explorer and via properties unblock all GitNoob files.", "GitNoob");
                        return;

                    default:
                        {
                            var result = MessageBox.Show("The blocked status of the executable \"" + executableFileName + "\" could not be retrieved." + Environment.NewLine + blocked.Exception.Message + Environment.NewLine + Environment.NewLine + "Continue ?", "GitNoob", MessageBoxButtons.YesNo);
                            if (result != DialogResult.Yes) return;
                        }
                        break;
                }
            }

            string rootConfigurationFilename = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "GitNoob", "GitNoob.ini");
            if (args.Length >= 1)
            {
                rootConfigurationFilename = Path.GetFullPath(args[0]);
            }

            if (!File.Exists(rootConfigurationFilename))
            {
                MessageBox.Show("Root configuration file does not exist:" + Environment.NewLine + rootConfigurationFilename, "GitNoob");
                return;
            }

            string singleInstanceMutexName;
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] hash = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rootConfigurationFilename));

                singleInstanceMutexName = "GitNoob." + BitConverter.ToString(hash).Replace("-", string.Empty);
            }

            bool firstInstance = false;
            using (Mutex singleInstanceMutex = new Mutex(true, singleInstanceMutexName, out firstInstance))
            {
                if (firstInstance)
                {
                    Config.Loader.ProjectTypeLoader.LoadProjectTypesAssembly(Path.Combine(programPath, "GitNoob.ProjectTypes.dll"));

                    List<Config.IConfig> configs = new List<Config.IConfig>();
                    try
                    {
                        configs.Add(new Config.Loader.IniFileLoader(rootConfigurationFilename, programPath));
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error loading configuration." + Environment.NewLine + Environment.NewLine + ex.Message, "GitNoob configuration error");
                        return;
                    }
                    if (!CheckConfigs(configs)) return;

                    Gui.Program.Utils.Resources.setIcon("error", Properties.Resources.error);

                    Gui.Program.Utils.Resources.setIcon("git gui", Properties.Resources.breeze_icons_5_81_0_git_gui);
                    Gui.Program.Utils.Resources.setIcon("gitk", Properties.Resources.list_symbol_of_three_items_with_dots_icon_icons_com_72994_modified);
                    Gui.Program.Utils.Resources.setIcon("get latest", Properties.Resources.download);
                    Gui.Program.Utils.Resources.setIcon("merge", Properties.Resources.merge);
                    Gui.Program.Utils.Resources.setIcon("delete all changes", Properties.Resources.delete_all_changes);
                    Gui.Program.Utils.Resources.setIcon("git repair", Properties.Resources.wrench_951);

                    Gui.Program.Utils.Resources.setIcon("clear cache", Properties.Resources.clear_cache);
                    Gui.Program.Utils.Resources.setIcon("ngrok", Properties.Resources.ngrok_black);
                    Gui.Program.Utils.Resources.setIcon("open logfiles", Properties.Resources.log_file);
                    Gui.Program.Utils.Resources.setIcon("delete logfiles", Properties.Resources.log_file_delete);
                    Gui.Program.Utils.Resources.setIcon("open configfiles", Properties.Resources.nullset);

                    string licenseText = String.Empty;
                    try
                    {
                        var asm = Assembly.GetExecutingAssembly();
                        var names = asm.GetManifestResourceNames();

                        using (var stream = asm.GetManifestResourceStream("GitNoob.LICENSE.md"))
                        {
                            using (var reader = new StreamReader(stream, Encoding.UTF8))
                            {
                                licenseText = reader.ReadToEnd();
                            }

                        }
                    }
                    catch { }


                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new Gui.Forms.ChooseProjectForm(configs, programPath, licenseText));
                }
                else
                {
                    try
                    {
                        foreach (Process proc in Process.GetProcessesByName(me.ProcessName))
                        {
                            if (proc.Id != me.Id && proc.Modules[0].FileName == executableFileName)
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
