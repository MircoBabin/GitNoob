using System;
using System.Collections.Generic;

namespace GitNoob.Git.Command.Tag
{
    public class ListTags : Command
    {
        public Dictionary<string, GitTag> result { get; private set; }

        public ListTags(GitWorkingDirectory gitworkingdirectory) : base(gitworkingdirectory)
        {
            result = null;

            RunGit("list", "for-each-ref --color=never \"--format=%(*objectname)%1f%(objecttype)%1f%(refname)%1f%(refname:short)%1f%1f%(contents)%1e\" refs/tags");
        }

        protected override void RunGitDone()
        {
            var executor = GetGitExecutor("list");

            result = new Dictionary<string, GitTag>();
            foreach (var line in executor.Output.Trim().Split('\u001e'))
            {
                var parts = line.Trim().Split('\u001f');
                if (parts.Length >= 5)
                {
                    string commit = parts[0].Trim().ToLowerInvariant();
                    string type = parts[1].Trim().ToLowerInvariant();
                    string fullname = parts[2].Trim();
                    string shortname = parts[3].Trim();

                    string message = parts[5].Trim();
                    message = message.Replace("\r", "").Replace("\n", Environment.NewLine);

                    if (type == "commit")
                    {
                        result.Add(fullname, new GitTag(fullname, shortname, GitTag.TagType.LightWeight, commit, message));
                    }
                    else if (type == "tag")
                    {
                        result.Add(fullname, new GitTag(fullname, shortname, GitTag.TagType.Annotated, commit, message));
                    }
                }
            }
        }
    }
}
