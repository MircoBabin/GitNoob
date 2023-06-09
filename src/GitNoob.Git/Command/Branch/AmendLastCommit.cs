using GitNoob.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace GitNoob.Git.Command.Branch
{
    public class AmendLastCommit : Command
    {
        //public bool? result { get; private set; }

        public AmendLastCommit(GitWorkingDirectory gitworkingdirectory, DateTime? authorTime, DateTime? commitTime) : base(gitworkingdirectory)
        {
            //result = null;

            StringBuilder options = new StringBuilder();
            Dictionary<string, string> environmentVariables = new Dictionary<string, string>();

            if (authorTime != null && authorTime.HasValue)
            {
                options.Append(" \"--date=" + GitUtils.FormatDateTimeForGit(authorTime.Value) + "\"");
            }

            if (commitTime != null && commitTime.HasValue)
            {
                environmentVariables.Add("GIT_COMMITTER_DATE", GitUtils.FormatDateTimeForGit(commitTime.Value));
            }

            RunGit("amend", "commit --amend --quiet --no-edit" + options.ToString(), null, environmentVariables);
        }

        protected override void RunGitDone()
        {
            var executor = GetGitExecutor("amend");
            //result can not be determined
        }
    }
}
