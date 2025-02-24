using System.Text;

namespace GitNoob.Git.Command.Tag
{
    public class CreateTagToLastCommitOfBranch : Command
    {
        //public bool? result { get; private set; }

        public CreateTagToLastCommitOfBranch(GitWorkingDirectory gitworkingdirectory, string branchNameOrNullForCurrentBranch, string tagname, string message) : base(gitworkingdirectory)
        {
            //result = null;

            // commitmessage via file
            var tagMessageFilename = System.IO.Path.GetTempFileName();
            var encoding = new UTF8Encoding(false);
            System.IO.File.WriteAllBytes(tagMessageFilename, encoding.GetBytes(message != null ? message : string.Empty));

            ExecutorGit executor;
            if (branchNameOrNullForCurrentBranch == null)
            {
                //create on current branch
                executor = RunGit("tag", new string[] { "tag", "--annotate", "--no-sign", "--force", "--file=" + tagMessageFilename, tagname });
            }
            else
            {
                executor = RunGit("tag", new string[] { "tag", "--annotate", "--no-sign", "--force", "--file=" + tagMessageFilename, tagname, branchNameOrNullForCurrentBranch });
            }
            executor.WaitFor();

            try
            {
                System.IO.File.Delete(tagMessageFilename);
            }
            catch { }
        }

        protected override void RunGitDone()
        {
            var executor = GetGitExecutor("tag");
            //result can not be determined
        }
    }
}
