using System;

namespace GitNoob.Gui.Program.Action.Step
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

            var message = new MessageWithLinks("Unpack last commit failed.");

            if (result.ErrorRebaseInProgress || result.ErrorMergeInProgress)
            {
                FailureRemedy = new Remedy.MessageRebasingOrMerging(this, message, result.ErrorRebaseInProgress, result.ErrorMergeInProgress);
                return false;
            }

            if (result.ErrorDetachedHead)
            {
                FailureRemedy = new Remedy.MessageDetachedHead(this, message);
                return false;
            }

            if (result.ErrorWorkingTreeChanges || result.ErrorStagedUncommittedFiles)
            {
                FailureRemedy = new Remedy.MessageChanges(this, message, result.ErrorWorkingTreeChanges, result.ErrorStagedUncommittedFiles);
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

