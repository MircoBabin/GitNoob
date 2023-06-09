namespace GitNoob.Git.Command.Branch
{
    public class CommitAllChanges : Command
    {
        public string commitid { get; private set; }

        public CommitAllChanges(GitWorkingDirectory gitworkingdirectory, string commitMessage) : base(gitworkingdirectory)
        {
            commitid = null;

            string before_commitid;
            {
                var executor = new GetLastCommitOfBranch(gitworkingdirectory, "HEAD");
                executor.WaitFor();

                before_commitid = executor.commitid;
            }

            {
                var executor = RunGit("add", "add --all");
                executor.WaitFor();
            }

            if (!string.IsNullOrWhiteSpace(commitMessage))
            {
                {
                    var executor = RunGit("commit", "commit --quiet --message \"" + commitMessage + "\"");
                    executor.WaitFor();
                }

                {
                    var executor = new GetLastCommitOfBranch(gitworkingdirectory, "HEAD");
                    executor.WaitFor();

                    if (executor.commitid != before_commitid)
                    {
                        commitid = executor.commitid;
                    }
                }
            }
        }

        protected override void RunGitDone()
        {
        }
    }
}
