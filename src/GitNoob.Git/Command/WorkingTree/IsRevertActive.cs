using System.IO;

namespace GitNoob.Git.Command.WorkingTree
{
    public class IsRevertActive : Command
    {
        public bool? result { get; private set; }

        public IsRevertActive(GitWorkingDirectory gitworkingdirectory) : base(gitworkingdirectory)
        {
            result = null;

            //Check for .git/REVERT_HEAD file existance
            RunCommand("file", new ResolveGitPath(gitworkingdirectory, "REVERT_HEAD"));
        }

        protected override void RunGitDone()
        {
            var file = (ResolveGitPath) GetCommand("file");
            if (File.Exists(file.result))
            {
                result = true;
                return;
            }

            result = false;
        }
    }
}
