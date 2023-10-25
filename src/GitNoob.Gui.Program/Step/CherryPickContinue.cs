using GitNoob.Gui.Visualizer;

namespace GitNoob.Gui.Program.Step
{
    public class CherryPickContinue : Step
    {
        public CherryPickContinue() : base() { }

        protected override bool run()
        {
            BusyMessage = "Busy - continuing cherry pick";

            var result = StepsExecutor.Config.Git.CherryPickContinue();

            var message = new VisualizerMessageWithLinks("Continuing cherry pick failed.");

            if (result.IsGitDisasterHappening != false)
            {
                FailureRemedy = new Remedy.MessageGitDisaster(this, message, result);
                return false;
            }

            if (result.ErrorNotCherryPicking)
            {
                return true;
            }

            if (result.ErrorConflicts)
            {
                FailureRemedy = new Remedy.ResolveCherryPickConflicts(this, message);
                return false;
            }

            if (!result.CherryPicked)
            {
                FailureRemedy = new Remedy.MessageUnknownResult(this, message, result);
                return false;
            }

            return true;
        }
    }
}
