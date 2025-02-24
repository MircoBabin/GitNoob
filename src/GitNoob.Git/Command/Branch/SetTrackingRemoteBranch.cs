namespace GitNoob.Git.Command.Branch
{
    public class SetTrackingRemoteBranch : Command
    {
        public SetTrackingRemoteBranch(GitWorkingDirectory gitworkingdirectory, string branchName, string remoteName, string remoteBranch) : base(gitworkingdirectory)
        {
            RunGit("set", new string[] { "branch", "--set-upstream-to=" + remoteName + "/" + remoteBranch, branchName });
        }

        protected override void RunGitDone()
        {
            var executor = GetGitExecutor("set");
            //result can't be determined
        }
    }
}
