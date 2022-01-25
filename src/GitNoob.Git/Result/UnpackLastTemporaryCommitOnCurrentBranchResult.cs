using System;

namespace GitNoob.Git.Result
{
    public class UnpackLastTemporaryCommitOnCurrentBranchResult
    {
        public string CurrentBranch { get; set; }
        public bool Unpacked { get; set; }
        public bool NoTemporaryCommitToUnpack { get; set; }

        public bool ErrorDetachedHead { get; set; }
        public bool ErrorStagedUncommittedFiles { get; set; }
        public bool ErrorWorkingTreeChanges { get; set; }
        public bool ErrorRebaseInProgress { get; set; }
        public bool ErrorMergeInProgress { get; set; }
        public bool ErrorUnpacking { get; set; }

        public UnpackLastTemporaryCommitOnCurrentBranchResult()
        {
            CurrentBranch = String.Empty;
            Unpacked = false;
            NoTemporaryCommitToUnpack = false;

            ErrorDetachedHead = false;
            ErrorStagedUncommittedFiles = false;
            ErrorWorkingTreeChanges = false;
            ErrorRebaseInProgress = false;
            ErrorMergeInProgress = false;
            ErrorUnpacking = false;
        }
    }
}
