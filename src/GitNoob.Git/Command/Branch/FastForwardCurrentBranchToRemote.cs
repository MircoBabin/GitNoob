namespace GitNoob.Git.Command.Branch
{
    public class FastForwardCurrentBranchToRemote : Command
    {
        public bool? result { get; private set; }

        public FastForwardCurrentBranchToRemote(GitWorkingDirectory gitworkingdirectory) : base(gitworkingdirectory)
        {
            result = null;

            RunGit("merge", "merge --ff-only");
        }

        protected override void RunGitDone()
        {
            var executor = GetGitExecutor("merge");
            result = string.IsNullOrWhiteSpace(executor.Error.Trim());
        }
    }
}
