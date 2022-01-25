using System;

namespace GitNoob.Git.Result
{
    public class ChangeCurrentBranchResult
    {
        public bool Changed { get; set; }
        public string CurrentBranch { get; set; }

        public bool ErrorStagedUncommittedFiles { get; set; }
        public bool ErrorWorkingTreeChanges { get; set; }
        public bool ErrorRebaseInProgress { get; set; }
        public bool ErrorMergeInProgress { get; set; }
        public bool ErrorChanging { get; set; }

        public ChangeCurrentBranchResult()
        {
            Changed = false;
            CurrentBranch = String.Empty;

            ErrorStagedUncommittedFiles = false;
            ErrorWorkingTreeChanges = false;
            ErrorRebaseInProgress = false;
            ErrorMergeInProgress = false;
            ErrorChanging = false;
        }
    }
}
