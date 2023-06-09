using GitNoob.Gui.Visualizer;

namespace GitNoob.Gui.Program.Step
{
    public class AskDeleteAllChanges : Step
    {
        public AskDeleteAllChanges() : base() { }

        protected override bool run()
        {
            //not really a failure, but a solution to ask for confirmation
            var message = new VisualizerMessageWithLinks();

            FailureRemedy = new Remedy.InputConfirmDeleteAllChanges(this, message);
            return false;
        }
    }
}
