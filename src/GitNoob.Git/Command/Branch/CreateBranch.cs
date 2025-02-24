namespace GitNoob.Git.Command.Branch
{
    public class CreateBranch : Command
    {
        //public bool? result { get; private set; }

        public CreateBranch(GitWorkingDirectory gitworkingdirectory, string newBranch, string branchFromBranchName, bool checkout) : base(gitworkingdirectory)
        {
            //result = null;

            if (checkout)
            {
                RunGit("branch", new string[] { "checkout", "-b", newBranch, branchFromBranchName });
            }
            else
            {
                RunGit("branch", new string[] { "branch", newBranch, branchFromBranchName });
            }
        }

        protected override void RunGitDone()
        {
            var executor = GetGitExecutor("branch");
            //result can not be determined
        }
    }
}
