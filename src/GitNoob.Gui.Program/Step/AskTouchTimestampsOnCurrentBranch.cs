using GitNoob.Gui.Visualizer;

namespace GitNoob.Gui.Program.Step
{
    public class AskTouchTimestampsOnCurrentBranch : Step
    {
        public AskTouchTimestampsOnCurrentBranch() : base() { }

        protected override bool run()
        {
            BusyMessage = "Busy - retrieving status";

            var message = new VisualizerMessageWithLinks();

            var result = StepsExecutor.Config.Git.CheckForGitDisaster();
            if (result.IsGitDisasterHappening != false)
            {
                FailureRemedy = new Remedy.MessageGitDisaster(this, message, result);
                return false;
            }

            //not really a failure, but a solution to ask for confirmation

            FailureRemedy = new Remedy.MessageConfirmTouchTimestampsOnCurrentBranch(this, message, result.GitDisaster_CurrentBranchShortName);
            return false;
        }
    }
}
