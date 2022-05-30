using System.IO;

namespace GitNoob.Git.Command.WorkingTree
{
    public class IsCherryPickActive : Command
    {
        public bool? result { get; private set; }

        public IsCherryPickActive(GitWorkingDirectory gitworkingdirectory) : base(gitworkingdirectory)
        {
            result = null;

            //Check for .git/CHERRY_PICK_HEAD file existance
            RunCommand("file", new ResolveGitPath(gitworkingdirectory, "CHERRY_PICK_HEAD"));
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
