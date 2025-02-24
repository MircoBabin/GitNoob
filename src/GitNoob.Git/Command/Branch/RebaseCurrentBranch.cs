namespace GitNoob.Git.Command.Branch
{
    public class RebaseCurrentBranch : Command
    {
        //public bool? result { get; private set; }

        public RebaseCurrentBranch(GitWorkingDirectory gitworkingdirectory, string ontoBranch) : base(gitworkingdirectory)
        {
            //result = null;

            RunGit("rebase", new string[] { "rebase", ontoBranch });
        }

        protected override void RunGitDone()
        {
            var executor = GetGitExecutor("rebase");
            //result can't be determined
        }
    }
}
