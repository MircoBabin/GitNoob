using GitNoob.Gui.Visualizer;

namespace GitNoob.Gui.Program.Step
{
    public class AskTouchTimestampsOnCurrentBranch : Step
    {
        public AskTouchTimestampsOnCurrentBranch() : base() { }

        protected override bool run()
        {
            BusyMessage = "Busy - retrieving status";

            var result = StepsExecutor.Config.Git.RetrieveStatus();
            var message = new VisualizerMessageWithLinks();

            if (result.Rebasing || result.Merging)
            {
                FailureRemedy = new Remedy.MessageRebasingOrMerging(this, message, result.Rebasing, result.Merging);
                return false;
            }

            if (result.DetachedHead_NotOnBranch)
            {
                FailureRemedy = new Remedy.MessageDetachedHead(this, message);
                return false;
            }

            if (result.HasWorkingTreeChanges || result.HasStagedUncommittedFiles)
            {
                FailureRemedy = new Remedy.MessageChanges(this, message, result.HasWorkingTreeChanges, result.HasStagedUncommittedFiles);
                return false;
            }

            //not really a failure, but a solution to ask for confirmation

            FailureRemedy = new Remedy.MessageConfirmTouchTimestampsOnCurrentBranch(this, message, result.CurrentBranch);
            return false;
        }
    }
}
