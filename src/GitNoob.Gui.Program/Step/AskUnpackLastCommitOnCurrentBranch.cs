using GitNoob.Gui.Visualizer;

namespace GitNoob.Gui.Program.Step
{
    public class AskUnpackLastCommitOnCurrentBranch : Step
    {
        public AskUnpackLastCommitOnCurrentBranch() : base() { }

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

            FailureRemedy = new Remedy.MessageConfirmUnpackLastCommitOnCurrentBranch(this, message, result.GitDisaster_CurrentBranchShortName, result.CurrentBranchLastCommitId, result.CurrentBranchLastCommitMessage);
            return false;
        }
    }
}
