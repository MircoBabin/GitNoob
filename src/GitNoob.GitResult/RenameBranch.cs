namespace GitNoob.GitResult
{
    public class RenameBranchResult : BaseGitDisasterResult
    {
        public bool Renamed { get; set; }

        public bool ErrorRenaming { get; set; }

        public RenameBranchResult() : base()
        {
            Renamed = false;
            ErrorRenaming = false;
        }
    }
}
