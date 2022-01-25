using System;

namespace GitNoob.Git.Command.Repository
{
    public class RemoteReachable : Command
    {
        public bool? result { get; private set; }

        public RemoteReachable(GitWorkingDirectory gitworkingdirectory) : base(gitworkingdirectory)
        {
            result = null;

            if (!String.IsNullOrWhiteSpace(_gitworkingdirectory.RemoteUrl))
            {
                RunGit("connect", "ls-remote --quiet \"" + _gitworkingdirectory.RemoteUrl + "\"");
            }
            else
            {
                result = true;
            }
        }

        protected override void RunGitDone()
        {
            if (result == null)
            {
                var connect = GetGitExecutor("connect");
                result = (connect.ExitCode == 0);
            }
        }
    }
}
