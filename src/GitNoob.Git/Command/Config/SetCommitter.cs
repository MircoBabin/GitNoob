using System;

namespace GitNoob.Git.Command.Config
{
    public class SetCommitter : Command
    {
        //public bool? result { get; private set; }

        public SetCommitter(GitWorkingDirectory gitworkingdirectory, string name, string email) : base(gitworkingdirectory)
        {
            RunGit("name", "config user.name \"" + name + "\"");
            RunGit("email", "config user.email \"" + email + "\"");
        }

        protected override void RunGitDone()
        {
            //result can't be determined
        }
    }
}
