namespace GitNoob.Gui.Program.Action.Step
{
    public class DeleteCurrentBranch : Step
    {
        private string _branchName;

        public DeleteCurrentBranch(string BranchName) : base()
        {
            _branchName = BranchName;
        }

        protected override bool run()
        {
            BusyMessage = "Busy - deleting current branch \"" + _branchName + "\"";

            {
                var result = StepsExecutor.Config.Git.DeleteCurrentBranch(_branchName);

                var message = new MessageWithLinks("Deleting current branch failed.");

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
