using System;

namespace GitNoob.Git.Result
{
    public class MergeResult
    {
        public string CurrentBranch { get; set; }

        public bool Merged { get; set; }
        public bool Aborted { get; set; }

        public bool ErrorDetachedHead { get; set; }
        public bool ErrorStagedUncommittedFiles { get; set; }
        public bool ErrorWorkingTreeChanges { get; set; }
        public bool ErrorUnpushedCommitsOnMainBranch { get; set; }
        public bool ErrorRebaseInProgress { get; set; }
        public bool ErrorMergeInProgress { get; set; }
        public bool ErrorRetrievingLastCommit { get; set; }
        public bool ErrorConflicts { get; set; }

        public bool ErrorNotMerging { get; set; }

        public MergeResult()
        {
            CurrentBranch = String.Empty;

            Merged = false;
            Aborted = false;

            ErrorDetachedHead = false;
            ErrorStagedUncommittedFiles = false;
            ErrorWorkingTreeChanges = false;
            ErrorUnpushedCommitsOnMainBranch = false;
            ErrorRebaseInProgress = false;
            ErrorMergeInProgress = false;
            ErrorRetrievingLastCommit = false;
            ErrorConflicts = false;

            ErrorNotMerging = false;
        }
    }
}

