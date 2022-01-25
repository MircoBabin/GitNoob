namespace GitNoob.Git.Command.Branch
{
    public class GetLastCommitOfBranch : Command
    {
        public string commitid { get; private set; }
        public string commitmessage { get; private set; }

        public GetLastCommitOfBranch(GitWorkingDirectory gitworkingdirectory, string localBranch) : base(gitworkingdirectory)
        {
            commitid = null;
            commitmessage = null;

            RunGit("commit", "rev-parse --verify \"" + localBranch + "\"");
        }

        protected override void RunGitDone()
        {
            var executor = GetGitExecutor("commit");
            commitid = executor.Output.Trim();
            if (!string.IsNullOrWhiteSpace(commitid))
            {
                var msg = RunGit("message", "rev-list --no-commit-header --format=%B --max-count=1 \"" + commitid + "\"");
                msg.WaitFor();

                commitmessage = msg.Output.Trim();
            }
        }
    }
}
