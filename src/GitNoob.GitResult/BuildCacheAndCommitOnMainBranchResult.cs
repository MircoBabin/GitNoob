namespace GitNoob.GitResult
{
    public class BuildCacheAndCommitOnMainBranchResult : BaseGitDisasterResult
    {
        public Config.IProjectType_ActionResult BuildCache { get; set; }
        public bool Updated { get; set; }
        public bool NotUpdatedBecauseNothingChanged { get; set; }

        public bool ErrorBuildingCache { get; set; }
        public bool ErrorChangingToMainBranch { get; set; }
        public bool ErrorCommittingChanges { get; set; }

        public BuildCacheAndCommitOnMainBranchResult() : base()
        {
            BuildCache = null; 
            Updated = false;
            NotUpdatedBecauseNothingChanged = false;

            ErrorBuildingCache = false; //When true: the current branch is now the main branch - can't restore current branch as there maybe working tree changes or staged uncommitted files
            ErrorChangingToMainBranch = false;
            ErrorCommittingChanges = false; //When true: the current branch is now the main branch - can't restore current branch as there are working tree changes or staged uncommitted files
        }
    }
}
