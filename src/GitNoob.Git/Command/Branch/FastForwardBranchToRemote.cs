namespace GitNoob.Git.Command.Branch
{
    public class FastForwardBranchToRemote : Command
    {
        public bool? result { get; private set; }

        public FastForwardBranchToRemote(GitWorkingDirectory gitworkingdirectory, Result.GitBranch branch) : base(gitworkingdirectory)
        {
            result = null;

            //fast forward a branch without checking out
            RunGit("ff", "fetch . \"" + branch.RemoteBranchFullName + ":" + branch.FullName + "\"");
        }

        protected override void RunGitDone()
        {
            var executor = GetGitExecutor("ff");
            result = string.IsNullOrWhiteSpace(executor.Error.Trim());
        }
    }
}
