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
                if (branchFromBranchName != null)
                    RunGit("branch", new string[] { "checkout", "-b", newBranch, branchFromBranchName });
                else
                    RunGit("branch", new string[] { "checkout", "-b", newBranch}); // on current commit
            }
            else
            {
                if (branchFromBranchName != null)
                    RunGit("branch", new string[] { "branch", newBranch, branchFromBranchName });
                else
                    RunGit("branch", new string[] { "branch", newBranch }); //on current commit
            }
        }

        protected override void RunGitDone()
        {
            var executor = GetGitExecutor("branch");
            //result can not be determined
        }
    }
}
