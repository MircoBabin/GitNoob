using System;

namespace GitNoob.GitResult
{
    public class TouchCommitAndAuthorTimestampsOfCurrentBranchResult : BaseGitDisasterResult
    {
        public bool NoCommitsToTouch { get; set; }
        public bool Touched { get; set; }
        public uint NumberOfTouchedCommits { get; set; }

        public bool ErrorCurrentBranchIsMainBranch { get; set; }
        public bool ErrorCurrentBranchIsTrackingRemoteBranch { get; set; }
        public bool ErrorNoCommonCommitWithMainBranch { get; set; }

        public bool ErrorCreatingTemporaryBranch { get; set; }
        public bool ErrorCreatingSafetyTag { get; set; }
        public bool ErrorCherryPickingCommit { get; set; }

        public TouchCommitAndAuthorTimestampsOfCurrentBranchResult() : base()
        {
            NoCommitsToTouch = false;
            Touched = false;
            NumberOfTouchedCommits = 0;

            ErrorCurrentBranchIsMainBranch = false;
            ErrorCurrentBranchIsTrackingRemoteBranch = false;
            ErrorNoCommonCommitWithMainBranch = false;

            ErrorCreatingTemporaryBranch = false;
            ErrorCreatingSafetyTag = false;
            ErrorCherryPickingCommit = false;
        }
    }
}
