namespace GitNoob.GitResult
{
    public class BaseGitDisasterResult
    {
        public GitBranch GitDisaster_CurrentGitBranch { get; set; }
        public string GitDisaster_CurrentBranchShortName { get; set; }

        public bool? IsGitDisasterHappening { get; set; }

        public bool? GitDisaster_DetachedHead { get; set; }
        public bool? GitDisaster_StagedUncommittedFiles { get; set; }
        public bool? GitDisaster_WorkingTreeChanges { get; set; }
        public bool? GitDisaster_UnpushedCommitsOnMainBranch { get; set; }

        public bool? GitDisaster_RebaseInProgress { get; set; }
        public bool? GitDisaster_MergeInProgress { get; set; }
        public bool? GitDisaster_CherryPickInProgress { get; set; }
        public bool? GitDisaster_RevertInProgress { get; set; }

        protected BaseGitDisasterResult()
        {
            GitDisaster_CurrentGitBranch = null;
            GitDisaster_CurrentBranchShortName = string.Empty;

            IsGitDisasterHappening = null;

            GitDisaster_DetachedHead = null;
            GitDisaster_StagedUncommittedFiles = null;
            GitDisaster_WorkingTreeChanges = null;
            GitDisaster_UnpushedCommitsOnMainBranch = null;

            GitDisaster_RebaseInProgress = null;
            GitDisaster_MergeInProgress = null;
            GitDisaster_CherryPickInProgress = null;
            GitDisaster_RevertInProgress = null;
        }
    }
}
