namespace GitNoob.GitResult
{
    public class DeleteWorkingTreeChangesAndStagedUncommittedFilesResult
    {
        public bool ErrorRebaseInProgress { get; set; }
        public bool ErrorMergeInProgress { get; set; }

        public bool ErrorStillStagedUncommittedFiles { get; set; }
        public bool ErrorStillWorkingTreeChanges { get; set; }

        public bool ChangesDeleted { get; set; }

        public DeleteWorkingTreeChangesAndStagedUncommittedFilesResult()
        {
            ErrorRebaseInProgress = false;
            ErrorMergeInProgress = false;
            ErrorStillStagedUncommittedFiles = false;
            ErrorStillWorkingTreeChanges = false;

            ChangesDeleted = false;
        }
    }
}

