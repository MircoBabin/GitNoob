namespace GitNoob.Git.Result
{
    public class CommitAllChangesOnCurrentBranchResult
    {
        public bool ErrorDetachedHead { get; set; }
        public bool ErrorStagedUncommittedFiles { get; set; }
        public bool ErrorRebaseInProgress { get; set; }
        public bool ErrorMergeInProgress { get; set; }

        public bool Committed { get; set; }

        public CommitAllChangesOnCurrentBranchResult()
        {
            ErrorDetachedHead = false;
            ErrorStagedUncommittedFiles = false;
            ErrorRebaseInProgress = false;
            ErrorMergeInProgress = false;

            Committed = false;
        }
    }
}

