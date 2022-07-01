namespace GitNoob.Gui.Program.Action.Step
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

            if (result.ErrorNotRebasing)
            {
                return true;
            }

            if (result.ErrorRebaseInProgress)
            {
                FailureRemedy = new Remedy.MessageRebasingOrMerging(this, message, result.ErrorRebaseInProgress, result.ErrorMergeInProgress);
                return false;
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

