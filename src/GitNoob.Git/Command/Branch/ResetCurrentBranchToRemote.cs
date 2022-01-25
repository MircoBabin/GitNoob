namespace GitNoob.Git.Command.Branch
{
    public class ResetCurrentBranchToRemote : Command
    {
        public ResetCurrentBranchToRemote(GitWorkingDirectory gitworkingdirectory) : base(gitworkingdirectory)
        {
            RunGit("reset", "reset --hard @{upstream}");
        }

        protected override void RunGitDone()
        {
        }
    }
}
