using System;

namespace GitNoob.Git.Result
{
    public class TouchCommitAndAuthorTimestampsOfCurrentBranchResult
    {
        public string CurrentBranch { get; set; }

        public bool NoCommitsToTouch { get; set; }
        public bool Touched { get; set; }
        public uint NumberOfTouchedCommits { get; set; }

        public bool ErrorStagedUncommittedFiles { get; set; }
        public bool ErrorWorkingTreeChanges { get; set; }
        public bool ErrorRebaseInProgress { get; set; }
        public bool ErrorMergeInProgress { get; set; }

        public bool ErrorCurrentBranchIsMainBranch { get; set; }
        public bool ErrorCurrentBranchIsTrackingRemoteBranch { get; set; }
        public bool ErrorDetachedHead { get; set; }
        public bool ErrorNoCommonCommitWithMainBranch { get; set; }

        public bool ErrorCreatingTemporaryBranch { get; set; }
        public bool ErrorCreatingSafetyTag { get; set; }
        public bool ErrorCherryPickingCommit { get; set; }

        public TouchCommitAndAuthorTimestampsOfCurrentBranchResult()
        {
            CurrentBranch = String.Empty;

            NoCommitsToTouch = false;
            Touched = false;
            NumberOfTouchedCommits = 0;

            ErrorStagedUncommittedFiles = false;
            ErrorWorkingTreeChanges = false;
            ErrorRebaseInProgress = false;
            ErrorMergeInProgress = false;

            ErrorCurrentBranchIsMainBranch = false;
            ErrorCurrentBranchIsTrackingRemoteBranch = false;
            ErrorDetachedHead = false;
            ErrorNoCommonCommitWithMainBranch = false;

            ErrorCreatingTemporaryBranch = false;
            ErrorCreatingSafetyTag = false;
            ErrorCherryPickingCommit = false;
        }
    }
}
