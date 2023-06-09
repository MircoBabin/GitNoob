using System;

namespace GitNoob.Git.Result
{
    public class UnpackLastCommitOnCurrentBranchResult
    {
        public string CurrentBranch { get; set; }
        public bool Unpacked { get; set; }
        public bool NoCommitToUnpack { get; set; }

        public bool ErrorDetachedHead { get; set; }
        public bool ErrorStagedUncommittedFiles { get; set; }
        public bool ErrorWorkingTreeChanges { get; set; }
        public bool ErrorRebaseInProgress { get; set; }
        public bool ErrorMergeInProgress { get; set; }
        public bool ErrorUnpacking { get; set; }

        public UnpackLastCommitOnCurrentBranchResult()
        {
            CurrentBranch = String.Empty;
            Unpacked = false;
            NoCommitToUnpack = false;

            ErrorDetachedHead = false;
            ErrorStagedUncommittedFiles = false;
            ErrorWorkingTreeChanges = false;
            ErrorRebaseInProgress = false;
            ErrorMergeInProgress = false;
            ErrorUnpacking = false;
        }
    }
}
