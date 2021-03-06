using GitNoob.Gui.Program.Utils;
using System.Drawing;

namespace GitNoob.Gui.Program.Action
{
    public class StartGitGui : Action, IAction
    {
        public StartGitGui(StepsExecutor.StepConfig Config) : base(Config) { }

        private static string _cacheExecutable = null;
        public static string GetExecutable()
        {
            if (_cacheExecutable == null)
            {
                try
                {
                    _cacheExecutable = Utils.FileUtils.FindExePath("git-gui.exe");
                }
                catch
                {
                    _cacheExecutable = string.Empty;
                }
            }

            return _cacheExecutable;
        }

        public Icon icon()
        {
            return Utils.Resources.getIcon("git gui");
        }

        public void execute()
        {
            //direct execution, because also started as clickable link or button inside a Remedy
            //don't use StepsExecutor

            var executable = GetExecutable();
            if (string.IsNullOrEmpty(executable)) return;

            var batFile = new BatFile("run-gitgui", BatFile.RunAsType.runAsInvoker, BatFile.WindowType.hideWindow,
                stepConfig.Config.Project, stepConfig.Config.ProjectWorkingDirectory,
                stepConfig.Config.PhpIni);
            batFile.AppendLine("start \"Git-Gui\" \"" + executable + "\"");
            batFile.AppendLine("exit /b 0");

            batFile.Execute();
        }
    }
}

