namespace GitNoob.Git.Result
{
    public class EnsureMainBranchExistanceResult
    {
        public bool Exists { get; set; }

        public bool ErrorDetachedHead { get; set; }
        public bool ErrorStagedUncommittedFiles { get; set; }
        public bool ErrorWorkingTreeChanges { get; set; }
        public bool ErrorRebaseInProgress { get; set; }
        public bool ErrorMergeInProgress { get; set; }

        public bool ErrorNotAutomaticallyCreated { get; set; } //because there is no remote tracking branch named the same
        public bool ErrorNotTrackingRemoteBranch { get; set; }

        public EnsureMainBranchExistanceResult()
        {
            Exists = false;

            ErrorDetachedHead = false;
            ErrorStagedUncommittedFiles = false;
            ErrorWorkingTreeChanges = false;
            ErrorRebaseInProgress = false;
            ErrorMergeInProgress = false;

            ErrorNotAutomaticallyCreated = false;
            ErrorNotTrackingRemoteBranch = false;
        }
    }
}
