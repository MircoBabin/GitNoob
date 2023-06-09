using GitNoob.Gui.Visualizer;

namespace GitNoob.Gui.Program.Step
{
    public class EnsureMainBranchExistance : Step
    {
        private string _message;

        public EnsureMainBranchExistance(string message) : base()
        {
            _message = message;
        }

        protected override bool run()
        {
            BusyMessage = "Busy - ensuring main branch exists";

            var result = StepsExecutor.Config.Git.EnsureMainBranchExistance();

            var message = new VisualizerMessageWithLinks(_message);

            if (!result.Exists)
            {
                FailureRemedy = new Remedy.MessageUnknownResult(this, message, result);
                return false;
            }

            return true;
        }
    }
}
