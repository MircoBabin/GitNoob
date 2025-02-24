namespace GitNoob.Git.Command.Config
{
    public class ClearCommitter : Command
    {
        //public bool? result { get; private set; }

        public ClearCommitter(GitWorkingDirectory gitworkingdirectory) : base(gitworkingdirectory)
        {
            //Sequentially, otherwise error: could not lock config file .git/config: File exists

            var configName = RunGit("name", new string[] { "config", "--unset", "user.name" });
            configName.WaitFor();

            var configEmail = RunGit("email", new string[] { "config", "--unset", "user.email" });
            configEmail.WaitFor();
        }

        protected override void RunGitDone()
        {
            //result can't be determined
        }
    }
}
