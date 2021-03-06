using GitNoob.Gui.Program.Utils;
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
                try
                {
                    _cacheExectable = FileUtils.FindExePath("%ComSpec%");
                }
                catch
                {
                    _cacheExectable = string.Empty;
                }
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
                phpPath = Config.ProjectWorkingDirectory.Php.Path.ToString();
                phpIniPath = Config.PhpIni.IniPath;
            }

            IExecutor = new ConfigIExecutor(
                this.NeedsPhp, 
                phpPath,
                phpIniPath,
                FileUtils.TempDirectoryForProject(Config.Project, Config.ProjectWorkingDirectory),
                Config.ProjectWorkingDirectory.Path.ToString(),
                Config.ProjectWorkingDirectory.Ngrok.AuthToken,
                Config.ProjectWorkingDirectory.Ngrok.ApiKey
            );
        }

        public Icon icon()
        {
            var cmd = GetExecutable();
            return Utils.ImageUtils.LoadIconForFile(cmd);
        }

        public void execute()
        {
            var executable = GetExecutable();
            if (string.IsNullOrEmpty(executable)) return;

            var info = new ProcessStartInfo
            {
                WorkingDirectory = Config.ProjectWorkingDirectory.Path.ToString(),
                FileName = executable,
                UseShellExecute = false,
                Arguments = "/K \"title " + FileUtils.DeriveFilename(String.Empty, Config.Project.Name) + "-" + Utils.FileUtils.DeriveFilename(String.Empty, Config.ProjectWorkingDirectory.Name) + "\"",
            };

            FileUtils.Execute_ProcessStartInfo(info, Config.ProjectWorkingDirectory, Config.PhpIni);
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
