using GitNoob.Utils;

namespace GitNoob.Gui.Program.Step
{
    public class StartGitGui : Step
    {
        public StartGitGui() : base()
        {
        }

        protected override bool run()
        {
            BusyMessage = "Busy - starting Git Gui";

            string executable = Action.StartGitGui.GetExecutable();
            if (string.IsNullOrWhiteSpace(executable)) return true;

            var batFile = new BatFile(StepsExecutor.Config.visualizerShowException, "run-gitk-current-branch", BatFile.RunAsType.runAsInvoker, BatFile.WindowType.hideWindow, "GitNoob - Git History",
                StepsExecutor.Config.Project, StepsExecutor.Config.ProjectWorkingDirectory,
                StepsExecutor.Config.PhpIni);
            batFile.AppendLine("start \"Git-Gui\" \"" + executable + "\"");
            batFile.AppendLine("exit /b 0");

            batFile.Start();

            return true;
        }
    }
}
