using System.Text;

namespace GitNoob.Git.Command.Branch
{
    public class CommitAllChanges : Command
    {
        public string commitid { get; private set; }

        public CommitAllChanges(GitWorkingDirectory gitworkingdirectory, string commitMessage) : base(gitworkingdirectory)
        {
            commitid = null;

            string before_commitid;
            {
                var executor = new GetLastCommitOfBranch(gitworkingdirectory, "HEAD");
                executor.WaitFor();

                before_commitid = executor.commitid;
            }

            {
                var executor = RunGit("add", new string[] { "add", "--all" });
                executor.WaitFor();
            }

            if (!string.IsNullOrWhiteSpace(commitMessage))
            {
                {
                    // commitmessage via file
                    var commitMessageFilename = System.IO.Path.GetTempFileName();
                    var encoding = new UTF8Encoding(false);
                    System.IO.File.WriteAllBytes(commitMessageFilename, encoding.GetBytes(commitMessage));

                    var executor = RunGit("commit", new string[] { "commit", "--quiet", "--file=" + commitMessageFilename });
                    executor.WaitFor();

                    try
                    {
                        System.IO.File.Delete(commitMessageFilename);
                    }
                    catch { }
                }

                {
                    var executor = new GetLastCommitOfBranch(gitworkingdirectory, "HEAD");
                    executor.WaitFor();

                    if (executor.commitid != before_commitid)
                    {
                        commitid = executor.commitid;
                    }
                }
            }
        }

        protected override void RunGitDone()
        {
        }
    }
}
