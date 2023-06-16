using System;

namespace GitNoob.GitResult
{
    public class RemoveLastTemporaryCommitOnCurrentBranchResult
    {
        public string CurrentBranch { get; set; }
        public bool Removed { get; set; }
        public bool NoCommitToRemove { get; set; }

        public bool ErrorDetachedHead { get; set; }
        public bool ErrorStagedUncommittedFiles { get; set; }
        public bool ErrorWorkingTreeChanges { get; set; }
        public bool ErrorRebaseInProgress { get; set; }
        public bool ErrorMergeInProgress { get; set; }
        public bool ErrorUnpacking { get; set; }

        public RemoveLastTemporaryCommitOnCurrentBranchResult()
        {
            CurrentBranch = String.Empty;
            Removed = false;
            NoCommitToRemove = false;

            ErrorDetachedHead = false;
            ErrorStagedUncommittedFiles = false;
            ErrorWorkingTreeChanges = false;
            ErrorRebaseInProgress = false;
            ErrorMergeInProgress = false;
            ErrorUnpacking = false;
        }
    }
}
