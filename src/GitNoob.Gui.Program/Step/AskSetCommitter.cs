using GitNoob.Gui.Visualizer;

namespace GitNoob.Gui.Program.Step
{
    public class AskSetCommitter : Step
    {
        private GitResult.StatusResult status;
        public AskSetCommitter(GitResult.StatusResult status) : base()
        {
            this.status = status;
        }

        protected override bool run()
        {
            //not really a failure, but a solution to ask for confirmation
            var message = new VisualizerMessageWithLinks();

            FailureRemedy = new Remedy.CommitName(this, message,
                status.CommitName, status.CommitEmail,
                StepsExecutor.Config.ProjectWorkingDirectory.Git.CommitName, StepsExecutor.Config.ProjectWorkingDirectory.Git.CommitEmail);
            return false;
        }
    }
}
