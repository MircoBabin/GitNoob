namespace GitNoob.Git.Command.Tag
{
    public class CreateTagToLastCommitOnCurrentBranch : Command
    {
        //public bool? result { get; private set; }

        public CreateTagToLastCommitOnCurrentBranch(GitWorkingDirectory gitworkingdirectory, string tagname, string message) : base(gitworkingdirectory)
        {
            //result = null;

            RunGit("tag", "tag --annotate --no-sign --force \"" + tagname + "\" \"--message=" + message + "\"");
        }

        protected override void RunGitDone()
        {
            var executor = GetGitExecutor("tag");
            //result can not be determined
        }
    }
}
