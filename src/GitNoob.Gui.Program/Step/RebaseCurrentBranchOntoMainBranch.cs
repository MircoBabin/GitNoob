using GitNoob.Gui.Visualizer;

namespace GitNoob.Gui.Program.Step
{
    public class RebaseCurrentBranchOntoMainBranch : Step
    {
        public RebaseCurrentBranchOntoMainBranch() : base() { }

        protected override bool run()
        {
            BusyMessage = "Busy - rebasing current branch onto main branch";

            var result = StepsExecutor.Config.Git.RebaseCurrentBranchOntoMainBranch();

            var message = new VisualizerMessageWithLinks("Rebasing current branch onto main branch failed.");

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

            if (result.ErrorConflicts)
            {
                FailureRemedy = new Remedy.ResolveRebaseConflicts(this, message, MainBranch, result.CurrentBranch);
                return false;
            }

            if (!result.Rebased)
            {
                FailureRemedy = new Remedy.MessageUnknownResult(this, message, result);
                return false;
            }

            return true;
        }
    }
}
