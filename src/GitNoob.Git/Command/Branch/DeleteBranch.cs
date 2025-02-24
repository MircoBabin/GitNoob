namespace GitNoob.Git.Command.Branch
{
    public class DeleteBranch : Command
    {
        //public bool? result { get; private set; }

        public DeleteBranch(GitWorkingDirectory gitworkingdirectory, string branch, bool forced) : base(gitworkingdirectory)
        {
            //result = null;

            RunGit("branch", new string[] { "branch", "--delete", (forced ? "--force" : null), branch });
        }

        protected override void RunGitDone()
        {
            var executor = GetGitExecutor("branch");
            //result can not be determined
        }
    }
}
