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

            if (amendWithAllCurrentChanges)
            {
                var executor = RunGit("add", new string[] { "add", "--all" });
                executor.WaitFor();
            }

            List<string> amendParms = new List<string>();
            Dictionary<string, string> amendEnvironmentVariables = new Dictionary<string, string>();

            amendParms.Add("commit");
            amendParms.Add("--amend");
            amendParms.Add("--quiet");
            amendParms.Add("--no-edit");

            if (authorTime != null && authorTime.HasValue)
            {
                amendParms.Add("--date=" + GitUtils.FormatDateTimeForGit(authorTime.Value));
            }

            if (commitTime != null && commitTime.HasValue)
            {
                amendEnvironmentVariables.Add("GIT_COMMITTER_DATE", GitUtils.FormatDateTimeForGit(commitTime.Value));
            }

            if (!string.IsNullOrWhiteSpace(commitMessage))
            {
                // commitmessage via file
                commitMessageFilename = System.IO.Path.GetTempFileName();
                var encoding = new UTF8Encoding(false);
                System.IO.File.WriteAllBytes(commitMessageFilename, encoding.GetBytes(commitMessage));
                amendParms.Add("--file=" + commitMessageFilename);
            }

            //Warning: staged files will also be updated in the previous commit!
            RunGit("amend", amendParms, null, amendEnvironmentVariables);
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
