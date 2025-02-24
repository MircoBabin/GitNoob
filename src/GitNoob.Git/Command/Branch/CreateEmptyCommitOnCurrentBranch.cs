using System.Text;

namespace GitNoob.Git.Command.Branch
{
    public class CreateEmptyCommitOnCurrentBranch : Command
    {
        //public bool? result { get; private set; }

        public CreateEmptyCommitOnCurrentBranch(GitWorkingDirectory gitworkingdirectory, string message) : base(gitworkingdirectory)
        {
            //result = null;

            // commitmessage via file
            var commitMessageFilename = System.IO.Path.GetTempFileName();
            var encoding = new UTF8Encoding(false);
            System.IO.File.WriteAllBytes(commitMessageFilename, encoding.GetBytes(message));

            var executor = RunGit("commit", new string[] { "commit", "--allow-empty", "--quiet", "--file=" + commitMessageFilename });
            executor.WaitFor();

            try
            {
                System.IO.File.Delete(commitMessageFilename);
            }
            catch { }
        }

        protected override void RunGitDone()
        {
            //result can not be determined
        }
    }
}
