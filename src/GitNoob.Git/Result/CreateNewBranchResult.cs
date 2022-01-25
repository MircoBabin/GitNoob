using System;

namespace GitNoob.Git.Result
{
    public class CreateNewBranchResult
    {
        public bool Created { get; set; }
        public string CurrentBranch { get; set; }

        public bool ErrorStagedUncommittedFiles { get; set; }
        public bool ErrorWorkingTreeChanges { get; set; }
        public bool ErrorRebaseInProgress { get; set; }
        public bool ErrorMergeInProgress { get; set; }
        public bool ErrorCreating { get; set; }

        public CreateNewBranchResult()
        {
            Created = false;
            CurrentBranch = String.Empty;

            ErrorStagedUncommittedFiles = false;
            ErrorWorkingTreeChanges = false;
            ErrorRebaseInProgress = false;
            ErrorMergeInProgress = false;
            ErrorCreating = false;
        }
    }
}
