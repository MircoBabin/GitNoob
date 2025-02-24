namespace GitNoob.Git.Command.Tag
{
    public class DeleteLocalTag : Command
    {
        //public bool? result { get; private set; }

        public DeleteLocalTag(GitWorkingDirectory gitworkingdirectory, string tagname) : base(gitworkingdirectory)
        {
            //result = null;

            RunGit("delete", new string[] { "tag", "-d", tagname });
        }

        protected override void RunGitDone()
        {
            var executor = GetGitExecutor("delete");
            //result can not be determined
        }
    }
}
