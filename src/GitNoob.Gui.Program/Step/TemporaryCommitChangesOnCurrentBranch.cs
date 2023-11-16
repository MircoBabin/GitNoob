using GitNoob.Gui.Visualizer;

namespace GitNoob.Gui.Program.Step
{
    public class TemporaryCommitChangesOnCurrentBranch : Step
    {
        public TemporaryCommitChangesOnCurrentBranch() : base() { }

        protected override bool run()
        {
            BusyMessage = "Busy - committing all changes into a temporary commit";

            var result = StepsExecutor.Config.Git.CommitAllChangesOnCurrentBranch(null);

            var message = new VisualizerMessageWithLinks("Committing all changes into a temporary commit is not possible.");

            if (result.IsGitDisasterHappening != false)
            {
                FailureRemedy = new Remedy.MessageGitDisaster(this, message, result);
                return false;
            }

            if (!result.Committed)
            {
                FailureRemedy = new Remedy.MessageUnknownResult(this, message, result);
                return false;
            }

            return true;
        }
    }
}
