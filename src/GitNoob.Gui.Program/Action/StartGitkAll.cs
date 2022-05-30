using GitNoob.Gui.Program.Utils;
using System.Drawing;

namespace GitNoob.Gui.Program.Action
{
    public class StartGitkAll : Action, IAction
    {
        public StartGitkAll(StepsExecutor.StepConfig Config) : base(Config) { }

        private static string _cacheExecutable = null;
        public static string GetExecutable()
        {
            if (_cacheExecutable == null)
            {
                _cacheExecutable = Utils.FileUtils.FindExePath("gitk.exe");
            }

            return _cacheExecutable;
        }

        public Icon icon()
        {
            return Utils.Resources.getIcon("gitk");
        }

        public void execute()
        {
            //direct execution, because also started as clickable link or button inside a Remedy
            //don't use StepsExecutor

            var batFile = new BatFile("run-gitkall", BatFile.RunAsType.runAsInvoker, BatFile.WindowType.hideWindow,
                stepConfig.Config.Project, stepConfig.Config.ProjectWorkingDirectory,
                stepConfig.Config.PhpIni);
            batFile.AppendLine("start \"Git-Gitk-All\" \"" + GetExecutable() + "\" --all");
            batFile.AppendLine("exit /b 0");

            batFile.Execute();
        }
    }
}

