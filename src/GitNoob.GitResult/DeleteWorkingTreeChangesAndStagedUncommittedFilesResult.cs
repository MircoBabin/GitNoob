namespace GitNoob.GitResult
{
    public class DeleteWorkingTreeChangesAndStagedUncommittedFilesResult : BaseGitDisasterResult
    {
        public bool ErrorStillStagedUncommittedFiles { get; set; }
        public bool ErrorStillWorkingTreeChanges { get; set; }

        public bool ChangesDeleted { get; set; }

        public DeleteWorkingTreeChangesAndStagedUncommittedFilesResult() : base()
        {
            ErrorStillStagedUncommittedFiles = false;
            ErrorStillWorkingTreeChanges = false;

            ChangesDeleted = false;
        }
    }
}

