using GitNoob.Gui.Visualizer;

namespace GitNoob.Gui.Program.Step
{
    public class CheckGitNoobTemporaryCommitsOnCurrentBranch : Step
    {
        private string _message;

        public CheckGitNoobTemporaryCommitsOnCurrentBranch(string message) : base()
        {
            _message = message;
        }

        protected override bool run()
        {
            BusyMessage = "Busy - checking for GitNoob Temporary Commits on current branch";

            var result = StepsExecutor.Config.Git.HasCurrentBranchUntilMainBranchGitNoobTemporaryCommits();

            var message = new VisualizerMessageWithLinks(_message);

            if (result.IsGitDisasterHappening != false)
            {
                FailureRemedy = new Remedy.MessageGitDisaster(this, message, result);
                return false;
            }

            if (result.HasGitNoobTemporaryCommit)
            {
                FailureRemedy = new Remedy.InputConfirmGitNoobTemporaryCommits(this, message, result.NumberOfGitNoobTemporaryCommits);
                return false;
            }
            
            if (!result.HasNoGitNoobTemporaryCommit)
            {
                FailureRemedy = new Remedy.MessageUnknownResult(this, message, result);
                return false;
            }

            return true;
        }
    }
}
