using GitNoob.GitResult;
using GitNoob.Utils;
using System;
using System.Collections.Generic;

namespace GitNoob.Git.Command.Repository
{
    public class ListReflog : Command
    {
        public List<GitReflog> result { get; private set; }

        public ListReflog(GitWorkingDirectory gitworkingdirectory) : base(gitworkingdirectory)
        {
            result = null;

            RunGit("reflog", "reflog --all --color=never \"--pretty=format:%H%x1f%gd%x1f%gscI%x1f%cI%x1f%B%x1e\"");
        }

        protected override void RunGitDone()
        {
            var executor = GetGitExecutor("reflog");

            result = new List<GitReflog>();
            foreach (var line in executor.Output.Trim().Split('\u001e'))
            {
                var parts = line.Trim().Split('\u001f');
                if (parts.Length >= 5)
                {
                    string commitId = parts[0].Trim().ToLowerInvariant();
                    string selectorShort = parts[1].Trim();
                    string message = parts[2].Trim();
                    string commitTimeIso8601 = parts[3].Trim();
                    string commitMessage = parts[4].Trim();

                    DateTime? commitTime = GitUtils.DecodeIso8601String(commitTimeIso8601);

                    result.Add(new GitReflog(commitId, selectorShort, message, commitTime, commitMessage));
                }
            }
        }
    }
}
