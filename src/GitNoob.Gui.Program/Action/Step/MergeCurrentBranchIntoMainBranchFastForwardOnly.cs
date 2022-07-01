namespace GitNoob.Gui.Program.Action.Step
{
    public class MergeCurrentBranchIntoMainBranchFastForwardOnly : Step
    {
        public MergeCurrentBranchIntoMainBranchFastForwardOnly() : base() { }

        protected override bool run()
        {
            BusyMessage = "Busy - merging into main branch (fast forward only)";

            var result = StepsExecutor.Config.Git.MergeFastForwardOnlyCurrentBranchIntoMainBranch();

            var message = new VisualizerMessageWithLinks("Merging into main branch (fast forward only) failed.");

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

            if (result.ErrorUnpushedCommitsOnMainBranch)
            {
                FailureRemedy = new Remedy.MoveChangesOnMainBranchToNewBranch(this, message, MainBranch);
                return false;
            }

            if (!result.Merged)
            {
                FailureRemedy = new Remedy.MessageUnknownResult(this, message, result);
                return false;
            }

            return true;
        }
    }
}
