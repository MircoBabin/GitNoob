namespace GitNoob.Gui.Program.Action.Step
{
    public class EnsureMainBranchExistance : Step
    {
        private string _message;

        public EnsureMainBranchExistance(string message) : base()
        {
            _message = message;
        }

        protected override bool run()
        {
            BusyMessage = "Busy - ensuring main branch exists";

            var result = StepsExecutor.Config.Git.EnsureMainBranchExistance();

            var message = new MessageWithLinks(_message);

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
            
            if (!result.Exists)
            {
                FailureRemedy = new Remedy.MessageUnknownResult(this, message, result);
                return false;
            }

            return true;
        }
    }
}
