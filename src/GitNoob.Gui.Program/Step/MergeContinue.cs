using GitNoob.Gui.Visualizer;

namespace GitNoob.Gui.Program.Step
{
    public class MergeContinue : Step
    {
        public MergeContinue() : base() { }

        protected override bool run()
        {
            BusyMessage = "Busy - continuing merge";

            var result = StepsExecutor.Config.Git.MergeContinue();

            var message = new VisualizerMessageWithLinks("Continuing merge failed.");

            if (result.IsGitDisasterHappening != false)
            {
                FailureRemedy = new Remedy.MessageGitDisaster(this, message, result);
                return false;
            }
            
            if (result.ErrorNotMerging)
            {
                return true;
            }

            if (result.ErrorConflicts)
            {
                FailureRemedy = new Remedy.ResolveMergeConflicts(this, message);
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
