using System.IO;

namespace GitNoob.Git.Command.WorkingTree
{
    public class IsMergeActive : Command
    {
        public bool? result { get; private set; }

        public IsMergeActive(GitWorkingDirectory gitworkingdirectory) : base(gitworkingdirectory)
        {
            result = null;
        
            //Check for .git/MERGE_HEAD file existance
            RunCommand("file", new ResolveGitPath(gitworkingdirectory, "MERGE_HEAD"));
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
