namespace GitNoob.Git.Result
{
    public class StatusResult
    {
        public bool DirectoryExists { get; set; }
        public bool IsGitRootDirectory { get; set; }
        public bool DetachedHead_NotOnBranch { get; set; }
        public string CurrentBranch { get; set; }
        public string CommitFullName { get; set; }
        public string CommitName { get; set; }
        public string CommitEmail { get; set; }
        public bool HasWorkingTreeChanges { get; set; }
        public bool HasStagedUncommittedFiles { get; set; }
        public bool Rebasing { get; set; }
        public bool Merging { get; set; }
        public bool Conflicts { get; set; }

        public StatusResult()
        {
            DirectoryExists = false;
            IsGitRootDirectory = false;
            DetachedHead_NotOnBranch = false;
            CurrentBranch = string.Empty;
            CommitFullName = string.Empty;
            CommitName = string.Empty;
            CommitEmail = string.Empty;
            HasWorkingTreeChanges = false;
            HasStagedUncommittedFiles = false;
            Rebasing = false;
            Merging = false;
            Conflicts = false;
        }
    }
}
