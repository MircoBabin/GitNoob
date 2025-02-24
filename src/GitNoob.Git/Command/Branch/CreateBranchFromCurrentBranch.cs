namespace GitNoob.Git.Command.Branch
{
    public class CreateBranchFromCurrentBranch : Command
    {
        //public bool? result { get; private set; }

        public CreateBranchFromCurrentBranch(GitWorkingDirectory gitworkingdirectory, string newBranch) : base(gitworkingdirectory)
        {
            //result = null;

            RunGit("branch", new string[] { "branch", newBranch });
        }

        protected override void RunGitDone()
        {
            var executor = GetGitExecutor("branch");
            //result can not be determined
        }
    }
}
