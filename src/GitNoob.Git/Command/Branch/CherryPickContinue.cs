
namespace GitNoob.Git.Command.Branch
{
    public class CherryPickContinue : Command
    {
        //public bool? result { get; private set; }

        public CherryPickContinue(GitWorkingDirectory gitworkingdirectory) : base(gitworkingdirectory)
        {
            //result = null;

            RunGit("cherry-pick", new string[] { "cherry-pick", "--continue" });
        }

        protected override void RunGitDone()
        {
            var executor = GetGitExecutor("cherry-pick");
            //result can't be determined
        }
    }
}
