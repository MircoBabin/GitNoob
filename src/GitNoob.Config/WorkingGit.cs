namespace GitNoob.Config
{
    public class WorkingGit
    {
        public string RemoteUrl { get; set; } //https://github.com/.../repository.git
        public string MainBranch { get; set; } //main

        public string CommitName { get; set; }
        public string CommitEmail { get; set; }

        public ConfigBoolean ClearCommitNameAndEmailOnExit { get; set; }
        public ConfigBoolean TouchTimestampOfCommitsBeforeMerge { get; set; }

        public WorkingGit()
        {
            MainBranch = "master";
            ClearCommitNameAndEmailOnExit = new ConfigBoolean(false);
            TouchTimestampOfCommitsBeforeMerge = new ConfigBoolean(false);
        }

        public void CopyFrom(WorkingGit other)
        {
            RemoteUrl = other.RemoteUrl;
            MainBranch = other.MainBranch;

            CommitName = other.CommitName;
            CommitEmail = other.CommitEmail;

            ClearCommitNameAndEmailOnExit.CopyFrom(other.ClearCommitNameAndEmailOnExit);
            TouchTimestampOfCommitsBeforeMerge.CopyFrom(other.TouchTimestampOfCommitsBeforeMerge);
        }

        public void useWorkingDirectory(WorkingDirectory WorkingDirectory)
        {
            ClearCommitNameAndEmailOnExit.useWorkingDirectory(WorkingDirectory);
            TouchTimestampOfCommitsBeforeMerge.useWorkingDirectory(WorkingDirectory);
        }
    }
}
