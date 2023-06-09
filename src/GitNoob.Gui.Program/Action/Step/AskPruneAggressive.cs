namespace GitNoob.Gui.Program.Action.Step
{
    public class AskStartMerge : Step
    {
        public AskStartMerge() : base() { }

        protected override bool run()
        {
            BusyMessage = "Busy - checking status";

            var result = StepsExecutor.Config.Git.RetrieveStatus();

            //not really a failure, but a solution to ask for confirmation
            var message = new VisualizerMessageWithLinks();

            FailureRemedy = new Remedy.MessageConfirmStartMerge(this, message, result.CurrentBranch, StepsExecutor.Config.Git.MainBranch, StepsExecutor.Config.ProjectWorkingDirectory.Git.RemoteUrl);
            return false;
        }
    }
}
