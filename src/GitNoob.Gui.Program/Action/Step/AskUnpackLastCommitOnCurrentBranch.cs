using System.Collections.Generic;

namespace GitNoob.Gui.Program.Action.Step
{
    public class AskUnpackLastCommitOnCurrentBranch : Step
    {
        public AskUnpackLastCommitOnCurrentBranch() : base() { }

        protected override bool run()
        {
            BusyMessage = "Busy - retrieving status";

            var result = StepsExecutor.Config.Git.RetrieveStatus();
            var message = new MessageWithLinks();

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

            FailureRemedy = new Remedy.MessageConfirmUnpackLastCommitOnCurrentBranch(this, message, result.CurrentBranch, result.CurrentBranchLastCommitId, result.CurrentBranchLastCommitMessage);
            return false;
        }
    }
}
