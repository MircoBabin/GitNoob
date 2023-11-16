namespace GitNoob.GitResult
{
    public class PruneResult : BaseGitDisasterResult
    {
        public bool Pruned { get; set; }

        public PruneResult() : base()
        {
            Pruned = false;
        }
    }
}
