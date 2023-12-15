using System;

namespace GitNoob.Git
{
    public class GitLockResult : GitResult.BaseGitDisasterResult
    {
        public bool Locked { get; set; }
        public bool Unlocked { get; set; }
        public GitLock GitLock { get; set; }

        public bool ErrorRetrievingRemoteBranchName { get; set; }
        public bool ErrorCreatingEmptyLockCommit { get; set; }
        public bool ErrorCreatingLockTag { get; set; }
        public bool ErrorLockNotAcquired { get; set; }
        public bool ErrorLockNotOwned { get; set; }

        public bool ErrorKeePassNotStarted { get; set; }
        public bool ErrorRemoteNotReachable { get; set; }
        public bool ErrorPushingLockTag { get; set; }
        public string PushOutput { get; set; }

        public DateTime? LockedTime { get; set; }
        public string LockedBy { get; set; }
        public string LockedMessage { get; set; }

        public GitLockResult() : base()
        {
            Locked = false;
            Unlocked = false;
            GitLock = null;

            ErrorRetrievingRemoteBranchName = false;
            ErrorCreatingEmptyLockCommit = false;
            ErrorCreatingLockTag = false;
            ErrorLockNotAcquired = false;
            ErrorLockNotOwned = false;

            ErrorKeePassNotStarted = false;
            ErrorRemoteNotReachable = false;
            ErrorPushingLockTag = false;
            PushOutput = String.Empty;

            LockedTime = null;
            LockedBy = String.Empty;
            LockedMessage = String.Empty;
        }
    }
}
