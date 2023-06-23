using GitNoob.Gui.Visualizer;

namespace GitNoob.Gui.Program.Step
{
    public class CreateBranchOnGitReferenceLog : Step
    {
        private GitResult.GitReflog _reflog;
        private string _branchName;

        public CreateBranchOnGitReferenceLog(GitResult.GitReflog reflog, string BranchName) : base()
        {
            _reflog = reflog;
            _branchName = BranchName;
        }

        protected override bool run()
        {
            BusyMessage = "Busy - creating new branch \"" + _branchName + "\"";

            {
                var result = StepsExecutor.Config.Git.CreateBranchOnGitReferenceLog(_reflog, _branchName, true);

                var message = new VisualizerMessageWithLinks("Creating new branch failed.");

                if (result.ErrorRebaseInProgress || result.ErrorMergeInProgress)
                {
                    FailureRemedy = new Remedy.MessageRebasingOrMerging(this, message, result.ErrorRebaseInProgress, result.ErrorMergeInProgress);
                    return false;
                }

                if (result.ErrorDetachedHead)
                {
                    FailureRemedy = new Remedy.MessageDetachedHead(this, message);
                    return false;
                }

                if (result.ErrorWorkingTreeChanges || result.ErrorStagedUncommittedFiles)
                {
                    FailureRemedy = new Remedy.MessageChanges(this, message, result.ErrorWorkingTreeChanges, result.ErrorStagedUncommittedFiles);
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
