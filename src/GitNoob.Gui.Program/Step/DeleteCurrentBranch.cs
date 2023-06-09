using GitNoob.Gui.Visualizer;

namespace GitNoob.Gui.Program.Step
{
    public class DeleteCurrentBranch : Step
    {
        private string _branchName;
        private string _message;

        public DeleteCurrentBranch(string BranchName, string Message) : base()
        {
            _branchName = BranchName;
            _message = Message;
        }

        protected override bool run()
        {
            BusyMessage = "Busy - deleting current branch \"" + _branchName + "\"";

            {
                var result = StepsExecutor.Config.Git.DeleteCurrentBranch(_branchName, _message);

                var message = new VisualizerMessageWithLinks("Deleting current branch failed.");

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

                StepsExecutor.CurrentBranchChangedTo(null);

                if (!result.Deleted)
                {
                    FailureRemedy = new Remedy.MessageUnknownResult(this, message, result);
                    return false;
                }
            }

            return true;
        }
    }
}
