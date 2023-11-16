namespace GitNoob.Git
{
    public class GitDisasterAllowed
    {
        public bool Allow_DetachedHead { get; set; }
        public bool Allow_StagedUncommittedFiles { get; set; }
        public bool Allow_WorkingTreeChanges { get; set; }
        public bool Allow_UnpushedCommitsOnMainBranch { get; set; }

        public bool Allow_RebaseInProgress { get; set; }
        public bool Allow_MergeInProgress { get; set; }
        public bool Allow_CherryPickInProgress { get; set; }
        public bool Allow_RevertInProgress { get; set; }

        public GitDisasterAllowed()
        {
            Allow_DetachedHead = false;
            Allow_StagedUncommittedFiles = false;
            Allow_WorkingTreeChanges = false;
            Allow_UnpushedCommitsOnMainBranch = false;

            Allow_RebaseInProgress = false;
            Allow_MergeInProgress = false;
            Allow_CherryPickInProgress = false;
            Allow_RevertInProgress = false;
        }

        public static GitDisasterAllowed AllowAll()
        {
            return new GitDisasterAllowed()
            {
                Allow_DetachedHead = true,
                Allow_StagedUncommittedFiles = true,
                Allow_WorkingTreeChanges = true,
                Allow_UnpushedCommitsOnMainBranch = true,

                Allow_RebaseInProgress = true,
                Allow_MergeInProgress = true,
                Allow_CherryPickInProgress = true,
                Allow_RevertInProgress = true,
            };
        }
    }
}
