namespace GitNoob.GitResult
{
    public class DeleteCurrentBranchResult
    {
        public bool Deleted { get; set; }

        public bool ErrorDetachedHead { get; set; }
        public bool ErrorStagedUncommittedFiles { get; set; }
        public bool ErrorWorkingTreeChanges { get; set; }
        public bool ErrorRebaseInProgress { get; set; }
        public bool ErrorMergeInProgress { get; set; }
        public bool ErrorCurrentBranchHasChanged { get; set; }
        public bool ErrorCannotDeleteMainBranch { get; set; }
        public bool ErrorCreatingSafetyTag { get; set; }
        public bool ErrorDeleting { get; set; }

        public DeleteCurrentBranchResult()
        {
            Deleted = false;

            ErrorDetachedHead = false;
            ErrorStagedUncommittedFiles = false;
            ErrorWorkingTreeChanges = false;
            ErrorRebaseInProgress = false;
            ErrorMergeInProgress = false;
            ErrorCurrentBranchHasChanged = false;
            ErrorCannotDeleteMainBranch = false;
            ErrorCreatingSafetyTag = false;
            ErrorDeleting = false;
        }
    }
}
