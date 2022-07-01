namespace GitNoob.Git.Command.Tag
{
    public class CreateTagToLastCommitOfBranch : Command
    {
        //public bool? result { get; private set; }

        public CreateTagToLastCommitOfBranch(GitWorkingDirectory gitworkingdirectory, string branchNameOrNullForCurrentBranch, string tagname, string message) : base(gitworkingdirectory)
        {
            //result = null;

            if (branchNameOrNullForCurrentBranch == null)
            {
                //create on current branch
                RunGit("tag", "tag --annotate --no-sign --force \"--message=" + (message != null ? message : string.Empty) + "\" \"" + tagname + "\"");
            }
            else
            {
                RunGit("tag", "tag --annotate --no-sign --force \"--message=" + (message != null ? message : string.Empty) + "\" \"" + tagname + "\" \"" + branchNameOrNullForCurrentBranch + "\"");
            }
        }

        protected override void RunGitDone()
        {
            var executor = GetGitExecutor("tag");
            //result can not be determined
        }
    }
}
