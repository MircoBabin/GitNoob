using System;

namespace GitNoob.Gui.Program.Action.Step
{
    public class LockReset : Step
    {
        private Git.GitLock _lock;

        public LockReset(Git.GitLock gitlock) : base()
        {
            _lock = gitlock;
        }

        protected override bool run()
        {
            BusyMessage = "Busy - resetting logical lock for branch \"" + _lock.branchName + "\"";

            var result = _lock.Reset();

            var message = new MessageWithLinks("Resetting Logical lock for branch \"" + _lock.branchName + "\" failed.");

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

            if (result.ErrorPushingLockTag)
            {
                message.Append(Environment.NewLine);
                message.Append("Error deleting lock tag.");

                FailureRemedy = new Remedy.MessageUnknownResult(this, message, result);
                return false;
            }

            if (!result.Unlocked)
            {
                FailureRemedy = new Remedy.MessageUnknownResult(this, message, result);
                return false;
            }

            if (StepsExecutor.GitLockStored == _lock) StepsExecutor.GitLockStored = null;
            return true;
        }
    }
}
