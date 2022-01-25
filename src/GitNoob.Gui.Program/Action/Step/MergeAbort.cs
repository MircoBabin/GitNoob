namespace GitNoob.Gui.Program.Action.Step
{
    public class MergeAbort : Step
    {
        private bool _presentRemedy;
        public MergeAbort(bool PresentRemedy) : base()
        {
            _presentRemedy = PresentRemedy;
        }

        protected override bool run()
        {
            BusyMessage = "Busy - aborting merge";

            var result = StepsExecutor.Config.Git.MergeAbort(null);

            var message = new MessageWithLinks("Aborting merge failed.");

            if (!_presentRemedy)
            {
                return true;
            }

            if (result.ErrorNotMerging)
            {
                return true;
            }

            if (result.ErrorMergeInProgress)
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

