namespace GitNoob.GitResult
{
    public class CommitAllChangesOnCurrentBranchResult : BaseGitDisasterResult
    {
        public bool Committed { get; set; }

        public CommitAllChangesOnCurrentBranchResult() : base()
        {
            Committed = false;
        }
    }
}

