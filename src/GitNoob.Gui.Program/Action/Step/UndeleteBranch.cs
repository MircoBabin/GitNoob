namespace GitNoob.Gui.Program.Action.Step
{
    public class UndeleteBranch : Step
    {
        private Git.GitDeletedBranch _deletedBranch;
        private string _branchName;

        public UndeleteBranch(Git.GitDeletedBranch DeletedBranch, string BranchName) : base()
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
