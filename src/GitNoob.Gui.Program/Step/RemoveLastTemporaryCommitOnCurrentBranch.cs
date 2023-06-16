using GitNoob.Gui.Visualizer;

namespace GitNoob.Gui.Program.Step
{
    public class RemoveLastTemporaryCommitOnCurrentBranch : Step
    {
        public RemoveLastTemporaryCommitOnCurrentBranch() : base()
        {
        }

        protected override bool run()
        {
            BusyMessage = "Busy - remove last temporary commit";

            var result = StepsExecutor.Config.Git.RemoveLastTemporaryCommitOnCurrentBranch();

            var message = new VisualizerMessageWithLinks("Remove last temporary commit failed.");

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


            if (!result.NoCommitToRemove && !result.Removed)
            {
                FailureRemedy = new Remedy.MessageUnknownResult(this, message, result);
                return false;
            }

            return true;
        }
    }
}

