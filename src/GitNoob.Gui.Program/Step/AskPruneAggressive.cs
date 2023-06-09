using GitNoob.Gui.Visualizer;

namespace GitNoob.Gui.Program.Step
{
    public class AskPruneAggressive : Step
    {
        public AskPruneAggressive() : base() { }

        protected override bool run()
        {
            //not really a failure, but a solution to ask for confirmation
            var message = new VisualizerMessageWithLinks();

            FailureRemedy = new Remedy.MessageConfirmPruneAggressive(this, message);
            return false;
        }
    }
}
