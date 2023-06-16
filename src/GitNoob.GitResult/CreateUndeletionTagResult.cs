namespace GitNoob.GitResult
{
    public class CreateUndeletionTagResult
    {
        public bool Created { get; set; }

        public bool ErrorDetachedHead { get; set; }
        public bool ErrorStagedUncommittedFiles { get; set; }
        public bool ErrorWorkingTreeChanges { get; set; }
        public bool ErrorRebaseInProgress { get; set; }
        public bool ErrorMergeInProgress { get; set; }
        public bool ErrorCurrentBranchHasChanged { get; set; }
        public bool ErrorCannotDeleteMainBranch { get; set; }
        public bool ErrorCreatingSafetyTag { get; set; }

        public CreateUndeletionTagResult()
        {
            Created = false;

            ErrorDetachedHead = false;
            ErrorStagedUncommittedFiles = false;
            ErrorWorkingTreeChanges = false;
            ErrorRebaseInProgress = false;
            ErrorMergeInProgress = false;
            ErrorCurrentBranchHasChanged = false;
            ErrorCannotDeleteMainBranch = false;
            ErrorCreatingSafetyTag = false;
        }
    }
}
