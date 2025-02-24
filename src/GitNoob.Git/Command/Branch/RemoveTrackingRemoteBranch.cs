namespace GitNoob.Git.Command.Branch
{
    public class RemoveTrackingRemoteBranch : Command
    {
        //public bool? result { get; private set; }

        public RemoveTrackingRemoteBranch(GitWorkingDirectory gitworkingdirectory, string localBranch) : base(gitworkingdirectory)
        {
            //result = null;

            RunGit("unset", new string[] { "branch", "--unset-upstream", localBranch });
        }

        protected override void RunGitDone()
        {
            var executor = GetGitExecutor("unset");
            //result can't be determined
        }
    }
}
