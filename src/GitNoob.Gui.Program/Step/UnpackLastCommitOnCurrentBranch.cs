using GitNoob.Gui.Visualizer;

namespace GitNoob.Gui.Program.Step
{
    public class UnpackLastCommitOnCurrentBranch : Step
    {
        public UnpackLastCommitOnCurrentBranch() : base()
        {
        }

        protected override bool run()
        {
            BusyMessage = "Busy - unpack last commit";

            var result = StepsExecutor.Config.Git.UnpackLastCommitOnCurrentBranch(Git.GitWorkingDirectory.UnpackLastCommitType.All);

            var message = new VisualizerMessageWithLinks("Unpack last commit failed.");

            if (result.IsGitDisasterHappening != false)
            {
                FailureRemedy = new Remedy.MessageGitDisaster(this, message, result);
                return false;
            }

            if (!result.NoCommitToUnpack && !result.Unpacked)
            {
                FailureRemedy = new Remedy.MessageUnknownResult(this, message, result);
                return false;
            }

            return true;
        }
    }
}

