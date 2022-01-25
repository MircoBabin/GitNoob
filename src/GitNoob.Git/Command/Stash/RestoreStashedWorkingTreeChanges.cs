using System;

namespace GitNoob.Git.Command.Stash
{
    public class RestoreStashedWorkingTreeChanges : Command
    {
        public bool? result { get; private set; }

        private string stashName;

        public RestoreStashedWorkingTreeChanges(GitWorkingDirectory gitworkingdirectory, string stashName) : base(gitworkingdirectory)
        {
            result = null;
            this.stashName = stashName;

            var list = new ListStashes(gitworkingdirectory);
            list.WaitFor();

            string stashno;
            list.result.TryGetValue(stashName, out stashno);
            if (stashno == null)
            {
                result = false;
                return;
            }

            RunGit("restore", "git stash pop \"" + stashno + "\"");
        }

        protected override void RunGitDone()
        {
            if (result == null)
            {
                var executor = GetGitExecutor("restore");
                result = String.IsNullOrWhiteSpace(executor.Error.Trim());
            }
        }
    }
}
