using GitNoob.GitResult;
using System.Collections.Generic;

namespace GitNoob.Git.Command.Remote
{
    public class ListRemotes : Command
    {
        public Dictionary<string, GitRemote> result { get; private set; }

        public ListRemotes(GitWorkingDirectory gitworkingdirectory) : base(gitworkingdirectory)
        {
            result = null;

            RunGit("list", "remote");
        }

        protected override void RunGitDone()
        {
            var list = GetGitExecutor("list");

            result = new Dictionary<string, GitRemote>();
            foreach (var line in list.Output.Trim().Split('\n'))
            {
                var name = line.Trim();
                if (!string.IsNullOrWhiteSpace(name))
                {
                    var listurl = RunGit("listurl-" + name, "remote get-url \"" + name + "\"");
                    listurl.WaitFor();

                    var url = listurl.Output.Trim();

                    result.Add(name, new GitRemote(name, url));
                }
            }
        }
    }
}
