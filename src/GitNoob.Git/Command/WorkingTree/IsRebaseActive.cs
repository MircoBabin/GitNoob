using System.IO;

namespace GitNoob.Git.Command.WorkingTree
{
    public class IsRebaseActive : Command
    {
        public bool? result { get; private set; }
        public Result.GitBranch currentBranch { get; private set; }
        public string ontoCommitid { get; private set; }
        public Result.GitBranch ontoBranch { get; private set; }

        public IsRebaseActive(GitWorkingDirectory gitworkingdirectory) : base(gitworkingdirectory)
        {
            result = null;
            currentBranch = null;
            ontoCommitid = null;
            ontoBranch = null;

            //Check for .git/rebase-merge and .git/rebase-apply directory existance
            RunCommand("path1", new ResolveGitPath(gitworkingdirectory, "rebase-merge"));
            RunCommand("path2", new ResolveGitPath(gitworkingdirectory, "rebase-apply"));
        }

        protected override void RunGitDone()
        {
            var path1 = (ResolveGitPath) GetCommand("path1");
            var path2 = (ResolveGitPath) GetCommand("path2");
            if (Directory.Exists(path1.result))
            {
                var filename = Path.Combine(path1.result, "head-name");
                if (File.Exists(filename))
                {
                    string fullname = File.ReadAllText(filename, System.Text.Encoding.UTF8).Trim();
                    currentBranch = new Result.GitBranch(fullname, Result.GitBranch.FullnameToShortname(fullname), Result.GitBranch.BranchType.Local);

                    filename = Path.Combine(path1.result, "onto");
                    ontoCommitid = File.ReadAllText(filename, System.Text.Encoding.UTF8).Trim();
                    if (!string.IsNullOrWhiteSpace(ontoCommitid))
                    {
                        var gitOntoBranchName = RunGit("branchname", "name-rev --name-only --exclude=tags/* \"" + ontoCommitid + "\"");
                        gitOntoBranchName.WaitFor();
                        string ontoBranch = gitOntoBranchName.Output.Trim();
                        if (!string.IsNullOrWhiteSpace(ontoBranch))
                        {
                            this.ontoBranch = new Result.GitBranch(ontoBranch, Result.GitBranch.FullnameToShortname(ontoBranch), Result.GitBranch.BranchType.Local);
                        }
                    }
                }

                result = true;
                return;
            }

            if (Directory.Exists(path2.result))
            {
                //git-am is active
                result = true;
                return;
            }

            result = false;
        }
    }
}
