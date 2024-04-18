namespace GitNoob.GitResult
{
    public class MoveUnpushedCommitsFromRemoteTrackingBranchToNewBranchResult : BaseGitDisasterResult
    {
        public bool Moved { get; set; }

        public bool ErrorNotTrackingRemoteBranch { get; set; }
        public bool ErrorRenaming { get; set; }
        public bool ErrorRemovingRemote { get; set; }

        public MoveUnpushedCommitsFromRemoteTrackingBranchToNewBranchResult() : base()
        {
            Moved = false;

            ErrorNotTrackingRemoteBranch = false;
            ErrorRenaming = false;
            ErrorRemovingRemote = false;
        }
    }
}
