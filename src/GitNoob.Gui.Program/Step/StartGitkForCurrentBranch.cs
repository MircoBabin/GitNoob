using GitNoob.Utils;

namespace GitNoob.Gui.Program.Step
{
    public class StartGitkForCurrentBranch : Step
    {
        private string _executable;
        public StartGitkForCurrentBranch(string executable) : base()
        {
            _executable = executable;
        }

        protected override bool run()
        {
            BusyMessage = "Busy - starting gitk";

            var result = StepsExecutor.Config.Git.RetrieveBranches();
            string currentBranch = result.CurrentBranch;

            var batFile = new BatFile(StepsExecutor.Config.visualizerShowException, "run-gitk-current-branch", BatFile.RunAsType.runAsInvoker, BatFile.WindowType.hideWindow, "GitNoob - Git History",
                StepsExecutor.Config.Project, StepsExecutor.Config.ProjectWorkingDirectory,
                StepsExecutor.Config.PhpIni);
            batFile.AppendLine("start \"Gitk\" \"" + _executable + "\" \"" + currentBranch + "\" \"" + MainBranch + "\" --");
            batFile.AppendLine("exit /b 0");

            batFile.Start();

            return true;
        }
    }
}
