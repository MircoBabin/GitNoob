namespace GitNoob.Git.Command.Config
{
    public class SetCommitter : Command
    {
        //public bool? result { get; private set; }

        public SetCommitter(GitWorkingDirectory gitworkingdirectory, string name, string email) : base(gitworkingdirectory)
        {
            //Sequentially, otherwise error: could not lock config file .git/config: File exists

            var configName = RunGit("name", "config user.name \"" + name + "\"");
            configName.WaitFor();

            var configEmail = RunGit("email", "config user.email \"" + email + "\"");
            configEmail.WaitFor();
        }

        protected override void RunGitDone()
        {
            //result can't be determined
        }
    }
}
