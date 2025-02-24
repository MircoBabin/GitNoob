namespace GitNoob.Git.Command.Branch
{
    public class GetRemoteBranch : Command
    {
        public string result { get; private set; }
        public string remoteName { get; private set; }
        public string remoteBranch { get; private set; }

        public GetRemoteBranch(GitWorkingDirectory gitworkingdirectory, string localBranch) : base(gitworkingdirectory)
        {
            result = null;
            remoteName = null;
            remoteBranch = null;

            RunGit("remote", new string[] { "rev-parse", "--abbrev-ref", "--symbolic-full-name", localBranch + "@{upstream}" });
        }

        protected override void RunGitDone()
        {
            var executor = GetGitExecutor("remote");
            result = executor.Output.Trim();
            if (!string.IsNullOrWhiteSpace(result))
            {
                var split = result.Split('/');
                if (split.Length == 2)
                {
                    remoteName = split[0];
                    remoteBranch = split[1];
                }
            }
        }
    }
}
