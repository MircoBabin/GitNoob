using GitNoob.Gui.Visualizer;

namespace GitNoob.Gui.Program.Step
{
    public class PruneAggressive : Step
    {
        public PruneAggressive() : base()
        {
        }

        protected override bool run()
        {
            BusyMessage = "Busy - empty git recycle bin" ;

            var result = StepsExecutor.Config.Git.PruneAggressive();

            var message = new VisualizerMessageWithLinks("Empty git recycle bin failed.");

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

            if (!result.Pruned)
            {
                FailureRemedy = new Remedy.MessageUnknownResult(this, message, result);
                return false;
            }

            return true;
        }
    }
}

