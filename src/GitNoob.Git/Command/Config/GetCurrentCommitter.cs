namespace GitNoob.Git.Command.Config
{
    public class GetCurrentCommitter : Command
    {
        public string result { get; private set; }
        public string name { get; private set; }
        public string email { get; private set; }

        public GetCurrentCommitter(GitWorkingDirectory gitworkingdirectory) : base(gitworkingdirectory)
        {
            result = null;

            RunGit("name", "config user.name");
            RunGit("email", "config user.email");
        }

        protected override void RunGitDone()
        {
            var name = GetGitExecutor("name");
            var email = GetGitExecutor("email");

            this.name = name.Output.Trim();
            this.email = email.Output.Trim();
            this.result = this.name + " <" + this.email + ">";
        }
    }
}
