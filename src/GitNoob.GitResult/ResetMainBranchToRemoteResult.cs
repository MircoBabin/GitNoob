namespace GitNoob.GitResult
{
    public class ResetMainBranchToRemoteResult : BaseGitDisasterResult
    {
        public bool Reset { get; set; }

        public bool ErrorCurrentBranchIsMainBranch { get; set; }
        public bool ErrorCurrentBranchCommitUnequalsMainBranchCommit { get; set; }

        public ResetMainBranchToRemoteResult() : base()
        {
            Reset = false;

            ErrorCurrentBranchIsMainBranch = false;
            ErrorCurrentBranchCommitUnequalsMainBranchCommit = false;
        }
    }
}
