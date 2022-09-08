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
            return ImageUtils.LoadIconForFile(cmd);
        }

        public void execute()
        {
            doExecute(false);
        }

        public void executeAsAdministrator()
        {
            doExecute(true);
        }

        private void doExecute(bool asAdministrator)
        {
            var executable = GetExecutable();
            if (string.IsNullOrEmpty(executable)) return;

            BatFile.StartDosPrompt(asAdministrator, stepConfig.Config.Project, stepConfig.Config.ProjectWorkingDirectory, stepConfig.Config.PhpIni);
        }
    }
}
