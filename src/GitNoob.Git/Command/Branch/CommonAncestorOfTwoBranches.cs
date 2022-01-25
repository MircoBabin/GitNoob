namespace GitNoob.Git.Command.Branch
{
    public class CommonAncestorOfTwoBranches : Command
    {
        public string commitid { get; private set; }

        public CommonAncestorOfTwoBranches(GitWorkingDirectory gitworkingdirectory, string mainBranch, string topicBranch) : base(gitworkingdirectory)
        {
            commitid = null;

            RunGit("base", "merge-base --fork-point \"" + mainBranch + "\" \"" + topicBranch + "\"");
        }

        protected override void RunGitDone()
        {
            var executor = GetGitExecutor("base");
            commitid = executor.Output.Trim();
        }
    }

}
