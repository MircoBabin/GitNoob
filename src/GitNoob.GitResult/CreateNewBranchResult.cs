using System;

namespace GitNoob.GitResult
{
    public class CreateNewBranchResult
    {
        public bool Created { get; set; }
        public string CurrentBranch { get; set; }

        public bool ErrorDetachedHead { get; set; }
        public bool ErrorStagedUncommittedFiles { get; set; }
        public bool ErrorWorkingTreeChanges { get; set; }
        public bool ErrorRebaseInProgress { get; set; }
        public bool ErrorMergeInProgress { get; set; }
        public bool ErrorBranchAlreadyExists { get; set; }
        public bool ErrorCreating { get; set; }

        public CreateNewBranchResult()
        {
            Created = false;
            CurrentBranch = String.Empty;

            ErrorDetachedHead = false;
            ErrorStagedUncommittedFiles = false;
            ErrorWorkingTreeChanges = false;
            ErrorRebaseInProgress = false;
            ErrorMergeInProgress = false;
            ErrorBranchAlreadyExists = false;
            ErrorCreating = false;
        }
    }
}
