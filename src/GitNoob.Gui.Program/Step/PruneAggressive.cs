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

            if (result.IsGitDisasterHappening != false)
            {
                FailureRemedy = new Remedy.MessageGitDisaster(this, message, result);
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

