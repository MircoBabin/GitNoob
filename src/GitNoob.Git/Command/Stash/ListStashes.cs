using System;
using System.Collections.Generic;

namespace GitNoob.Git.Command.Stash
{
    public class ListStashes : Command
    {
        public Dictionary<string, string> result;

        public ListStashes(GitWorkingDirectory gitworkingdirectory) : base(gitworkingdirectory)
        {
            result = null;

            RunGit("list", "git stash list");
        }

        protected override void RunGitDone()
        {
            var executor = GetGitExecutor("list");

            result = new Dictionary<string, string>();
            foreach (var line in executor.Output.Split('\n'))
            {
                var trimmed = line.Trim();
                if (trimmed.Length > 0)
                {
                    int p1 = trimmed.IndexOf(':');
                    if (p1 < 0) throw new Exception("first ':' not found - git stash list - expected format \"stash@{0}: On ...: Message\", got \"" + trimmed + "\"");

                    int p2 = trimmed.IndexOf(':', p1 + 1);
                    if (p2 < 0) throw new Exception("second ':' not found - git stash list - expected format \"stash@{0}: On ...: Message\", got \"" + trimmed + "\"");

                    string stashno = trimmed.Substring(0, p1).Trim();
                    string message = trimmed.Substring(p2 + 1).Trim();

                    result.Add(message, stashno);
                }
            }
        }
    }
}
