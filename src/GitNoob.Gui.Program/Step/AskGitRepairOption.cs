using GitNoob.Gui.Visualizer;

namespace GitNoob.Gui.Program.Step
{
    public class AskGitRepairOption : Step
    {
        public AskGitRepairOption() : base() { }

        protected override bool run()
        {
            //not really a failure, but a solution to ask for confirmation
            var message = new VisualizerMessageWithLinks();

            FailureRemedy = new Remedy.SelectGitRepairOption(this, message);
            return false;
        }
    }
}
