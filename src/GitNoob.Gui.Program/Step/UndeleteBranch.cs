using GitNoob.Gui.Visualizer;

namespace GitNoob.Gui.Program.Step
{
    public class UndeleteBranch : Step
    {
        private GitResult.GitDeletedBranch _deletedBranch;
        private string _branchName;

        public UndeleteBranch(GitResult.GitDeletedBranch DeletedBranch, string BranchName) : base()
        {
            _deletedBranch = DeletedBranch;
            _branchName = BranchName;
        }

        protected override bool run()
        {
            BusyMessage = "Busy - creating new branch \"" + _branchName + "\" from deleted branch \"" + _deletedBranch.BranchName + "\"";

            {
                var result = StepsExecutor.Config.Git.UndeleteBranch(_deletedBranch, _branchName, true);

                var message = new VisualizerMessageWithLinks("Creating new branch failed.");

                if (result.IsGitDisasterHappening != false)
                {
                    FailureRemedy = new Remedy.MessageGitDisaster(this, message, result);
                    return false;
                }

                if (result.ErrorBranchAlreadyExists)
                {
                    FailureRemedy = new Remedy.MessageBranchAlreadyExists(this, message, _branchName);
                    return false;
                }

                if (!result.Created)
                {
                    StepsExecutor.CurrentBranchChangedTo(null);
                    FailureRemedy = new Remedy.MessageUnknownResult(this, message, result);
                    return false;
                }

                StepsExecutor.CurrentBranchChangedTo(result.CurrentBranch);
            }

            return true;
        }
    }
}
