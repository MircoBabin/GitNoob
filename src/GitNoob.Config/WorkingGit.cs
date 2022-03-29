namespace GitNoob.Config
{
    public class WorkingGit
    {
        public string RemoteUrl { get; set; } //https://github.com/.../repository.git
        public string MainBranch { get; set; } //main

        public string CommitName { get; set; }
        public string CommitEmail { get; set; }

        public WorkingGit()
        {
            MainBranch = "master";
        }

        public void CopyFrom(WorkingGit other)
        {
            RemoteUrl = other.RemoteUrl;
            MainBranch = other.MainBranch;

            CommitName = other.CommitName;
            CommitEmail = other.CommitEmail;
        }

        public void useWorkingDirectory(WorkingDirectory WorkingDirectory)
        {
        }
    }
}
