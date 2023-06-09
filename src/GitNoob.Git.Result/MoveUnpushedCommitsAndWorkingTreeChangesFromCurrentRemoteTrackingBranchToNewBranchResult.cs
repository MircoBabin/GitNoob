using System;

namespace GitNoob.Git.Result
{
    public class MoveUnpushedCommitsAndWorkingTreeChangesFromCurrentRemoteTrackingBranchToNewBranchResult
    {
        public bool Moved { get; set; }
        public string CurrentBranch { get; set; }

        public bool ErrorDetachedHead { get; set; }
        public bool ErrorUnexpectedCurrentBranch { get; set; }
        public bool ErrorNotTrackingRemoteBranch { get; set; }
        public bool ErrorRenaming { get; set; }
        public bool ErrorRemovingRemote { get; set; }

        public MoveUnpushedCommitsAndWorkingTreeChangesFromCurrentRemoteTrackingBranchToNewBranchResult()
        {
            Moved = false;
            CurrentBranch = String.Empty;

            ErrorDetachedHead = false;
            ErrorUnexpectedCurrentBranch = false;
            ErrorNotTrackingRemoteBranch = false;
            ErrorRenaming = false;
            ErrorRemovingRemote = false;
        }
    }
}
