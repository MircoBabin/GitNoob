using System;

namespace GitNoob.Git.Result
{
    public class PruneResult
    {
        public bool Pruned { get; set; }

        public bool ErrorDetachedHead { get; set; }
        public bool ErrorStagedUncommittedFiles { get; set; }
        public bool ErrorWorkingTreeChanges { get; set; }
        public bool ErrorRebaseInProgress { get; set; }
        public bool ErrorMergeInProgress { get; set; }

        public PruneResult()
        {
            Pruned = false;

            ErrorDetachedHead = false;
            ErrorStagedUncommittedFiles = false;
            ErrorWorkingTreeChanges = false;
            ErrorRebaseInProgress = false;
            ErrorMergeInProgress = false;
        }
    }
}
