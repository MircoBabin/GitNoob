namespace GitNoob.Gui.Program.Action.Step
{
    public class CreateBranch : Step
    {
        private string _branchName;
        private string _branchFromBranchName;

        public CreateBranch(string BranchName, string BranchFromBranchName) : base()
        {
            _branchName = BranchName;
            _branchFromBranchName = BranchFromBranchName;
        }

        protected override bool run()
        {
            BusyMessage = "Busy - creating new branch \"" + _branchName + "\"";

            var result = StepsExecutor.Config.Git.CreateNewBranch(_branchName, _branchFromBranchName, true);

            var message = new MessageWithLinks("Creating new branch failed.");

            if (result.ErrorRebaseInProgress || result.ErrorMergeInProgress)
            {
                FailureRemedy = new Remedy.MessageRebasingOrMerging(this, message, result.ErrorRebaseInProgress, result.ErrorMergeInProgress);
                return false;
            }

            if (result.ErrorWorkingTreeChanges || result.ErrorStagedUncommittedFiles)
            {
                FailureRemedy = new Remedy.MessageChanges(this, message, result.ErrorWorkingTreeChanges, result.ErrorStagedUncommittedFiles);
                return false;
            }

            if (result.Created)
            {
                StepsExecutor.CurrentBranchChangedTo(result.CurrentBranch);
            }
            else
            {
                StepsExecutor.CurrentBranchChangedTo(null);
            }
            return true;
        }
    }
}
