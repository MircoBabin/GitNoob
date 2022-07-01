namespace GitNoob.Git.Command.Repository
{
    public class CommitsAreTheSame : Command
    {
        public bool? result { get; private set; } 

        public CommitsAreTheSame(GitWorkingDirectory gitworkingdirectory, string commitid1, string commitid2) : base(gitworkingdirectory)
        {
            result = null;
            if (commitid1 != commitid2)
            {
                RunGit("diff-tree", "diff-tree --color=never --no-renames --exit-code --quiet \"" + commitid1 + "\" \"" + commitid2 + "\"");
            }
            else
            {
                result = true;
            }
        }

        protected override void RunGitDone()
        {
            if (result == null)
            {
                var executor = GetGitExecutor("diff-tree");
                result = (executor.ExitCode == 0);
            }
        }
    }
}
