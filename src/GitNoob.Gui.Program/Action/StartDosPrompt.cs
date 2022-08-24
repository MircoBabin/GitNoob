using GitNoob.Gui.Program.Utils;
using System;
using System.Drawing;

namespace GitNoob.Gui.Program.Action
{
    public class StartDosPrompt : Action, IAction
    {
        public StartDosPrompt(StepsExecutor.StepConfig Config) : base(Config) { }

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

        public Icon icon()
        {
            var cmd = GetExecutable();
            return Utils.ImageUtils.LoadIconForFile(cmd);
        }

        public void execute()
        {
            var executable = GetExecutable();
            if (string.IsNullOrEmpty(executable)) return;

            var batfile = new BatFile("run-dosprompt", BatFile.RunAsType.runAsInvoker, BatFile.WindowType.showWindow, FileUtils.DeriveFilename(String.Empty, stepConfig.Config.Project.Name) + "-" + Utils.FileUtils.DeriveFilename(String.Empty, stepConfig.Config.ProjectWorkingDirectory.Name),
                stepConfig.Config.Project, stepConfig.Config.ProjectWorkingDirectory, stepConfig.Config.PhpIni);
            batfile.AppendLine("cmd /k");
            batfile.Start();
        }
    }
}
