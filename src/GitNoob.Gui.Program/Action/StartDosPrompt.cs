using System;
using System.Diagnostics;
using System.Drawing;

namespace GitNoob.Gui.Program.Action
{
    public class StartDosPrompt : IAction, Config.IExecutor
    {
        private static string _cacheExectable = null;
        private static string GetExecutable()
        {
            if (_cacheExectable == null)
            {
                _cacheExectable = Utils.FileUtils.FindExePath("%ComSpec%");
            }

            return _cacheExectable;
        }

        private ConfigIExecutor IExecutor;
        private ProgramWorkingDirectory Config;
        private bool NeedsPhp;
        public StartDosPrompt(ProgramWorkingDirectory Config)
        {
            this.Config = Config;
            this.NeedsPhp = Config.ProjectWorkingDirectory.ProjectType != null && Config.ProjectWorkingDirectory.ProjectType.Capabilities.NeedsPhp;

            string phpPath = null;
            string phpIniPath = null;
            if (this.NeedsPhp)
            {
                phpPath = Config.ProjectWorkingDirectory.Php.Path;
                phpIniPath = Config.PhpIni.IniPath;
            }

            IExecutor = new ConfigIExecutor(
                this.NeedsPhp, 
                phpPath,
                phpIniPath,
                Utils.FileUtils.TempDirectoryForProject(Config.Project, Config.ProjectWorkingDirectory),
                Config.ProjectWorkingDirectory.Path
            );
        }

        public Icon icon()
        {
            var cmd = GetExecutable();
            return Utils.ImageUtils.LoadIconForFile(cmd);
        }

        public void execute()
        {
            var info = new ProcessStartInfo
            {
                WorkingDirectory = Config.ProjectWorkingDirectory.Path,
                FileName = GetExecutable(),
                UseShellExecute = false,
            };

            if (NeedsPhp)
            {
                info.EnvironmentVariables["PHPRC"] = Config.PhpIni.IniPath; /* Directory containing php.ini */
                info.EnvironmentVariables["Path"] = info.EnvironmentVariables["Path"] + ";" + Config.ProjectWorkingDirectory.Php.Path;
            }

            Process.Start(info);
        }

        public string GetPhpExe()
        {
            if (!NeedsPhp) throw new Exception("ProjectType does not need Php");

            return IExecutor.GetPhpExe();
        }

        public Config.IExecutorResult ExecuteBatFile(string batFileContents, string commandline = null)
        {
            return IExecutor.ExecuteBatFile(batFileContents, commandline);
        }

        public void OpenBatFileInNewWindow(string batFileContents, string commandline = null)
        {
            IExecutor.OpenBatFileInNewWindow(batFileContents, commandline);
        }
    }
}
