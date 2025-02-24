namespace GitNoob.Git.Command.Branch
{
    public class RenameBranch : Command
    {
        public bool? result { get; private set; }
        public string error { get; private set; }

        public RenameBranch(GitWorkingDirectory gitworkingdirectory, string oldName, string newName) : base(gitworkingdirectory)
        {
            result = null;
            error = string.Empty;

            //rename a branch without checking out
            //
            //This moves unpushed commits and working tree changes on a current remote tracking branch to a new branch
            //After, the remote tracking branch has no local equivalent anymore. Not a problem, checkout the remote branch and it will be created again.
            RunGit("rename", new string[] { "branch", "-m", oldName, newName });
        }

        protected override void RunGitDone()
        {
            var executor = GetGitExecutor("rename");
            error = executor.Error.Trim();
            result = string.IsNullOrWhiteSpace(error);
        }
    }
}
