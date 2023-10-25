using GitNoob.Gui.Visualizer;

namespace GitNoob.Gui.Program.Step
{
    public class RebaseAbort : Step
    {
        private bool _presentRemedy;
        public RebaseAbort(bool PresentRemedy) : base()
        {
            _presentRemedy = PresentRemedy;
        }

        protected override bool run()
        {
            BusyMessage = "Busy - aborting rebase";

            var result = StepsExecutor.Config.Git.RebaseAbort();

            var message = new VisualizerMessageWithLinks("Aborting rebase failed.");

            if (!_presentRemedy)
            {
                return true;
            }

            if (result.IsGitDisasterHappening != false)
            {
                FailureRemedy = new Remedy.MessageGitDisaster(this, message, result);
                return false;
            }

            if (result.ErrorNotRebasing)
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

