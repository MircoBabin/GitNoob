using System;

namespace GitNoob.GitResult
{
    public class RebaseResult
    {
        public string CurrentBranch { get; set; }

        public bool Rebased { get; set; }
        public bool Aborted { get; set; }

        public bool ErrorDetachedHead { get; set; }
        public bool ErrorStagedUncommittedFiles { get; set; }
        public bool ErrorWorkingTreeChanges { get; set; }
        public bool ErrorUnpushedCommitsOnMainBranch { get; set; }
        public bool ErrorRebaseInProgress { get; set; }
        public bool ErrorMergeInProgress { get; set; }
        public bool ErrorConflicts { get; set; }

        public bool ErrorNotRebasing { get; set; }

        public RebaseResult()
        {
            CurrentBranch = String.Empty;

            Rebased = false;
            Aborted = false;

            ErrorDetachedHead = false;
            ErrorStagedUncommittedFiles = false;
            ErrorWorkingTreeChanges = false;
            ErrorUnpushedCommitsOnMainBranch = false;
            ErrorRebaseInProgress = false;
            ErrorMergeInProgress = false;
            ErrorConflicts = false;

            ErrorNotRebasing = false;
        }
    }
}
