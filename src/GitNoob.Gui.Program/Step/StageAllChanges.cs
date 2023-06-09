using GitNoob.Gui.Visualizer;

namespace GitNoob.Gui.Program.Step
{
    public class StageAllChanges : Step
    {
        public StageAllChanges() : base()
        {
        }

        protected override bool run()
        {
            BusyMessage = "Busy - staging all changes.";

            {
                var result = StepsExecutor.Config.Git.StageAllChangesOnCurrentBranch();

                var message = new VisualizerMessageWithLinks("Staging all changes failed.");

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

                if (!result.Staged)
                {
                    FailureRemedy = new Remedy.MessageUnknownResult(this, message, result);
                    return false;
                }
            }

            return true;
        }
    }
}
