namespace GitNoob.Git.Command.Remote
{
    public class ChangeUrl : Command
    {
        public ChangeUrl(GitWorkingDirectory gitworkingdirectory, string remoteName, string remoteUrl) : base(gitworkingdirectory)
        {
            var add = RunGit("add", new string[] { "remote", "add", remoteName, remoteUrl }); //may fail because remote already exists
            add.WaitFor();

            RunGit("change", new string[] { "remote", "set-url", remoteName, remoteUrl });
        }

        protected override void RunGitDone()
        {
            var change = GetGitExecutor("change");

            //result can't be determined
        }
    }
}
