namespace GitNoob.Git.Command.Branch
{
    public class FindCommonCommitOfTwoBranches : Command
    {
        public string commitid { get; private set; }

        public FindCommonCommitOfTwoBranches(GitWorkingDirectory gitworkingdirectory, string localBranch1, string localBranch2) : base(gitworkingdirectory)
        {
            commitid = null;

            RunGit("base", "merge-base \"" + localBranch1 + "\" \"" + localBranch2 + "\"");
        }

        protected override void RunGitDone()
        {
            var executor = GetGitExecutor("base");
            commitid = executor.Output.Trim();
        }
    }

}
