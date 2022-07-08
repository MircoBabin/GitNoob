namespace GitNoob.Gui.Program.Action.Step
{
    public class CreateBranchOntoMainBranch : Step
    {
        private string _branchName;

        public CreateBranchOntoMainBranch(string BranchName) : base()
        {
            _branchName = BranchName;
        }

        protected override bool run()
        {
            BusyMessage = "Busy - creating new branch \"" + _branchName + "\" onto main branch \"" + MainBranch + "\"";

            {
                var message = new VisualizerMessageWithLinks("Ensuring main branch existance failed.");

                var result = StepsExecutor.Config.Git.EnsureMainBranchExistance();
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
            }

            {
                var result = StepsExecutor.Config.Git.CreateNewBranch(_branchName, MainBranch, true);

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

                if (result.Created)
                {
                    StepsExecutor.CurrentBranchChangedTo(result.CurrentBranch);
                }
                else
                {
                    StepsExecutor.CurrentBranchChangedTo(null);
                }
            }

            return true;
        }
    }
}
