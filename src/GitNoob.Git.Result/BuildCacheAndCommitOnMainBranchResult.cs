namespace GitNoob.Git.Result
{
    public class BuildCacheAndCommitOnMainBranchResult
    {
        public Config.IProjectType_ActionResult BuildCache { get; set; }
        public bool Updated { get; set; }
        public bool NotUpdatedBecauseNothingChanged { get; set; }

        public bool ErrorDetachedHead { get; set; }
        public bool ErrorStagedUncommittedFiles { get; set; }
        public bool ErrorWorkingTreeChanges { get; set; }
        public bool ErrorRebaseInProgress { get; set; }
        public bool ErrorMergeInProgress { get; set; }
        public bool ErrorUnpushedCommitsOnMainBranch { get; set; }

        public bool ErrorBuildingCache { get; set; }
        public bool ErrorChangingToMainBranch { get; set; }
        public bool ErrorCommittingChanges { get; set; }

        public BuildCacheAndCommitOnMainBranchResult()
        {
            BuildCache = null; 
            Updated = false;
            NotUpdatedBecauseNothingChanged = false;

            ErrorDetachedHead = false;
            ErrorStagedUncommittedFiles = false;
            ErrorWorkingTreeChanges = false;
            ErrorRebaseInProgress = false;
            ErrorMergeInProgress = false;
            ErrorUnpushedCommitsOnMainBranch = false;

            ErrorBuildingCache = false; //When true: the current branch is now the main branch - can't restore current branch as there maybe working tree changes or staged uncommitted files
            ErrorChangingToMainBranch = false;
            ErrorCommittingChanges = false; //When true: the current branch is now the main branch - can't restore current branch as there are working tree changes or staged uncommitted files
        }
    }
}
