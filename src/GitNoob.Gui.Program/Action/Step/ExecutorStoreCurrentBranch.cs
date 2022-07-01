namespace GitNoob.Gui.Program.Action.Step
{
    public class ExecutorStoreCurrentBranch : Step
    {
        public ExecutorStoreCurrentBranch() : base() { }

        protected override bool run()
        {
            BusyMessage = "Busy - retrieving current branch";

            var result = StepsExecutor.Config.Git.RetrieveStatus();

            var message = new VisualizerMessageWithLinks("Storing current branch failed.");
            if (result.DetachedHead_NotOnBranch)
            {
                FailureRemedy = new Remedy.MessageDetachedHead(this, message);
                return false;
            }

            StepsExecutor.CurrentBranchStored = result.CurrentBranch;
            return true;
        }
    }
}
