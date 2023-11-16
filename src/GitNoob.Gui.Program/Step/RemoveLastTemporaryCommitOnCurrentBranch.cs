using GitNoob.Gui.Visualizer;

namespace GitNoob.Gui.Program.Step
{
    public class RemoveLastTemporaryCommitOnCurrentBranch : Step
    {
        public RemoveLastTemporaryCommitOnCurrentBranch() : base()
        {
        }

        protected override bool run()
        {
            BusyMessage = "Busy - remove last temporary commit";

            var result = StepsExecutor.Config.Git.RemoveLastTemporaryCommitOnCurrentBranch();

            var message = new VisualizerMessageWithLinks("Remove last temporary commit failed.");

            if (result.IsGitDisasterHappening != false)
            {
                FailureRemedy = new Remedy.MessageGitDisaster(this, message, result);
                return false;
            }


            if (!result.NoCommitToRemove && !result.Removed)
            {
                FailureRemedy = new Remedy.MessageUnknownResult(this, message, result);
                return false;
            }

            return true;
        }
    }
}

