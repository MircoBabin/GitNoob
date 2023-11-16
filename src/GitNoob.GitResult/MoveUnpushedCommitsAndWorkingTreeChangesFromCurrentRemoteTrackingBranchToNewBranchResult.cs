using System;

namespace GitNoob.GitResult
{
    public class MoveUnpushedCommitsAndWorkingTreeChangesFromCurrentRemoteTrackingBranchToNewBranchResult : BaseGitDisasterResult
    {
        public bool Moved { get; set; }
        public string CurrentBranch { get; set; }

        public bool ErrorUnexpectedCurrentBranch { get; set; }
        public bool ErrorNotTrackingRemoteBranch { get; set; }
        public bool ErrorRenaming { get; set; }
        public bool ErrorRemovingRemote { get; set; }

        public MoveUnpushedCommitsAndWorkingTreeChangesFromCurrentRemoteTrackingBranchToNewBranchResult() :base()
        {
            Moved = false;
            CurrentBranch = String.Empty;

            ErrorUnexpectedCurrentBranch = false;
            ErrorNotTrackingRemoteBranch = false;
            ErrorRenaming = false;
            ErrorRemovingRemote = false;
        }
    }
}
