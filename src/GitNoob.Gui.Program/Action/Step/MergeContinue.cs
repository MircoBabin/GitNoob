namespace GitNoob.Gui.Program.Action.Step
{
    public class MergeContinue : Step
    {
        public MergeContinue() : base() { }

        protected override bool run()
        {
            BusyMessage = "Busy - continuing merge";

            var result = StepsExecutor.Config.Git.MergeContinue();

            var message = new VisualizerMessageWithLinks("Continuing merge failed.");

            if (result.ErrorNotMerging)
            {
                return true;
            }

            if (result.ErrorConflicts)
            {
                FailureRemedy = new Remedy.ResolveMergeConflicts(this, message);
                return false;
            }

            if (!result.Merged)
            {
                FailureRemedy = new Remedy.MessageUnknownResult(this, message, result);
                return false;
            }

            return true;
        }
    }
}
