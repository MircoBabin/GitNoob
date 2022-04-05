namespace GitNoob.Gui.Program.Action.Step
{
    public class RenameBranch : Step
    {
        private string _branchName;
        private string _newName;

        public RenameBranch(string BranchName, string NewName) : base()
        {
            _branchName = BranchName;
            _newName = NewName;
        }

        protected override bool run()
        {
            BusyMessage = "Busy - renaming branch \"" + _branchName + "\" to \"" + _newName + "\"";

            {
                var result = StepsExecutor.Config.Git.RenameBranch(_branchName, _newName);

                var message = new MessageWithLinks("Renaming branch failed.");

                if (result.ErrorRebaseInProgress || result.ErrorMergeInProgress)
                {
                    FailureRemedy = new Remedy.MessageRebasingOrMerging(this, message, result.ErrorRebaseInProgress, result.ErrorMergeInProgress);
                    return false;
                }

                StepsExecutor.CurrentBranchChangedTo(null);
            }

            return true;
        }
    }
}
