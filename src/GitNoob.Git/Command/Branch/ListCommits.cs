using System.Collections.Generic;

namespace GitNoob.Git.Command.Branch
{
    public class ListCommits : Command
    {
        //Commits are in reversed order. Newest first, oldest last
        public List<GitCommit> result { get; private set; }

        public ListCommits(GitWorkingDirectory gitworkingdirectory, string afterCommitId, string uptoCommitId) : base(gitworkingdirectory)
        {
            result = null;

            RunGit("list", "rev-list --no-commit-header --format=%H%x1f%B%x1e \"" + afterCommitId + ".." + uptoCommitId + "\"");
        }

        protected override void RunGitDone()
        {
            result = new List<GitCommit>();

            var executor = GetGitExecutor("list");
            foreach (var line in executor.Output.Trim().Split('\u001e'))
            {
                var parts = line.Trim().Split('\u001f');
                if (parts.Length >= 2)
                {
                    string commitid = parts[0].Trim();
                    string commitmessage = parts[1].Trim();

                    result.Add(new GitCommit(commitid, commitmessage));
                }
            }
        }
    }
}
