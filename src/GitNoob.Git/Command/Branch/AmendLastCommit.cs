using GitNoob.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace GitNoob.Git.Command.Branch
{
    public class AmendLastCommit : Command
    {
        //public bool? result { get; private set; }

        private string commitMessageFilename = null;

        public AmendLastCommit(GitWorkingDirectory gitworkingdirectory, bool amendWithAllCurrentChanges, DateTime? authorTime, DateTime? commitTime, string commitMessage) : base(gitworkingdirectory)
        {
            //result = null;

            StringBuilder options = new StringBuilder();
            Dictionary<string, string> environmentVariables = new Dictionary<string, string>();

            if (amendWithAllCurrentChanges)
            {
                var executor = RunGit("add", "add --all");
                executor.WaitFor();
            }


            if (authorTime != null && authorTime.HasValue)
            {
                options.Append(" \"--date=" + GitUtils.FormatDateTimeForGit(authorTime.Value) + "\"");
            }

            if (commitTime != null && commitTime.HasValue)
            {
                environmentVariables.Add("GIT_COMMITTER_DATE", GitUtils.FormatDateTimeForGit(commitTime.Value));
            }

            if (!string.IsNullOrWhiteSpace(commitMessage))
            {
                // commitmessage via file
                commitMessageFilename = System.IO.Path.GetTempFileName();
                var encoding = new UTF8Encoding(false);
                System.IO.File.WriteAllBytes(commitMessageFilename, encoding.GetBytes(commitMessage));
                options.Append(" \"--file=" + commitMessageFilename + "\"");
            }

            //Warning: staged files will also be updated in the previous commit!
            RunGit("amend", "commit --amend --quiet --no-edit" + options.ToString(), null, environmentVariables);
        }

        protected override void RunGitDone()
        {
            if (!string.IsNullOrEmpty(commitMessageFilename))
            {
                try
                {
                    System.IO.File.Delete(commitMessageFilename);
                }
                catch { }
            }

            var executor = GetGitExecutor("amend");
            //result can not be determined
        }
    }
}
