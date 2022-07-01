using System;

namespace GitNoob.Gui.Program.Action.Step
{
    public class LockMainBranch : Step
    {
        private string _message;

        public LockMainBranch(string message) : base()
        {
            _message = message;
        }

        protected override bool run()
        {
            BusyMessage = "Busy - logical locking main branch";

            var status = StepsExecutor.Config.Git.RetrieveStatus();
            var result = StepsExecutor.Config.Git.AcquireGitLockForMainBranch(status.CommitFullName, _message);

            var message = new VisualizerMessageWithLinks("Logical locking main branch failed.");

            if (result.ErrorRebaseInProgress || result.ErrorMergeInProgress)
            {
                FailureRemedy = new Remedy.MessageRebasingOrMerging(this, message, result.ErrorRebaseInProgress, result.ErrorMergeInProgress);
                return false;
            }

            if (result.ErrorWorkingTreeChanges || result.ErrorStagedUncommittedFiles)
            {
                FailureRemedy = new Remedy.MessageChanges(this, message, result.ErrorWorkingTreeChanges, result.ErrorStagedUncommittedFiles);
                return false;
            }

            if (result.ErrorKeePassNotStarted)
            {
                FailureRemedy = new Remedy.MessageKeePassNotStarted(this, message);
                return false;
            }

            if (result.ErrorRemoteNotReachable)
            {
                FailureRemedy = new Remedy.MessageRemoteNotReachable(this, message, StepsExecutor.Config.Git.RemoteUrl);
                return false;
            }

            if (result.ErrorRetrievingRemoteBranchName ||
                result.ErrorCreatingEmptyLockCommit ||
                result.GitLock == null)
            {
                message.Append(Environment.NewLine);
                message.Append("Error creating lock tag.");

                FailureRemedy = new Remedy.MessageUnknownResult(this, message, result);
                return false;
            }

            if (result.ErrorCreatingLockTag || result.ErrorPushingLockTag || result.ErrorLockNotAcquired)
            {
                FailureRemedy = new Remedy.MessageLockNotAcquired(this, message, result);
                return false;
            }


            if (!result.Locked || result.GitLock == null)
            {
                FailureRemedy = new Remedy.MessageUnknownResult(this, message, result);
                return false;
            }

            StepsExecutor.GitLockStored = result.GitLock;

            return true;
        }
    }
}
