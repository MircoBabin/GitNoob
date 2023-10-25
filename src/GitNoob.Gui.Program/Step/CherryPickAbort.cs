using GitNoob.Gui.Visualizer;

namespace GitNoob.Gui.Program.Step
{
    public class CherryPickAbort : Step
    {
        private bool _presentRemedy;
        public CherryPickAbort(bool PresentRemedy) : base()
        {
            _presentRemedy = PresentRemedy;
        }

        protected override bool run()
        {
            BusyMessage = "Busy - aborting cherry pick";

            var result = StepsExecutor.Config.Git.CherryPickAbort();

            var message = new VisualizerMessageWithLinks("Aborting cherry pick failed.");

            if (!_presentRemedy)
            {
                return true;
            }

            if (result.IsGitDisasterHappening != false)
            {
                FailureRemedy = new Remedy.MessageGitDisaster(this, message, result);
                return false;
            }

            if (result.ErrorNotCherryPicking)
            {
                return true;
            }
            
            if (!result.Aborted)
            {
                FailureRemedy = new Remedy.MessageUnknownResult(this, message, result);
                return false;
            }

            return true;
        }
    }
}

