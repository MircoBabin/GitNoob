namespace GitNoob.Git.Command.Branch
{
    public class RebaseAbort : Command
    {
        //public bool? result { get; private set; }

        public RebaseAbort(GitWorkingDirectory gitworkingdirectory) : base(gitworkingdirectory)
        {
            //result = null;

            RunGit("rebase", new string[] { "rebase", "--abort" });
        }

        protected override void RunGitDone()
        {
            var executor = GetGitExecutor("rebase");
            //result can't be determined
        }
    }
}
