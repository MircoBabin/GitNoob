using GitNoob.Gui.Visualizer;

namespace GitNoob.Gui.Program.Step
{
    public class MergeCurrentBranchIntoMainBranchFastForwardOnly : Step
    {
        public MergeCurrentBranchIntoMainBranchFastForwardOnly() : base() { }

        protected override bool run()
        {
            BusyMessage = "Busy - merging into main branch (fast forward only)";

            var result = StepsExecutor.Config.Git.MergeFastForwardOnlyCurrentBranchIntoMainBranch();

            var message = new VisualizerMessageWithLinks("Merging into main branch (fast forward only) failed.");

            if (result.IsGitDisasterHappening != false)
            {
                FailureRemedy = new Remedy.MessageGitDisaster(this, message, result);
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
