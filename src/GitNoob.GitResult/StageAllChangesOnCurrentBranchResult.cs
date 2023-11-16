namespace GitNoob.GitResult
{
    public class StageAllChangesOnCurrentBranchResult : BaseGitDisasterResult
    {
        public bool Staged { get; set; }

        public StageAllChangesOnCurrentBranchResult() : base()
        {
            Staged = false;
        }
    }
}

