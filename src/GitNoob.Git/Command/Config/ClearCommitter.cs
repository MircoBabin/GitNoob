using System;

namespace GitNoob.Git.Command.Config
{
    public class ClearCommitter : Command
    {
        //public bool? result { get; private set; }

        public ClearCommitter(GitWorkingDirectory gitworkingdirectory) : base(gitworkingdirectory)
        {
            //Sequentially, otherwise error: could not lock config file .git/config: File exists

            var configName = RunGit("name", "config --unset user.name");
            configName.WaitFor();

            var configEmail = RunGit("email", "config --unset user.email");
            configEmail.WaitFor();
        }

        protected override void RunGitDone()
        {
            //result can't be determined
        }
    }
}
