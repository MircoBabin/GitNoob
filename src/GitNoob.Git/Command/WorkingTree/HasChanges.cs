namespace GitNoob.Git.Command.WorkingTree
{
    public class HasChanges : Command
    {
        public bool? workingtreeChanges { get; private set; }
        public bool? stagedUncommittedFiles { get; private set; }

        public HasChanges(GitWorkingDirectory gitworkingdirectory) : base(gitworkingdirectory)
        {
            workingtreeChanges = null;
            stagedUncommittedFiles = null;

            RunGit("workingtree-changes", "ls-files --exclude-standard --others --modified --deleted");
            RunGit("staged-but-uncommitted", "diff --cached --quiet");
        }

        protected override void RunGitDone()
        {
            {
                var cmd = GetGitExecutor("workingtree-changes");
                workingtreeChanges = (cmd.Output.Trim().Length > 0);
            }

            {
                var cmd = GetGitExecutor("staged-but-uncommitted");
                stagedUncommittedFiles = (cmd.ExitCode == 1);
            }
        }
    }
}

/*
https://newbedev.com/how-do-i-programmatically-determine-if-there-are-uncommitted-changes

require_clean_work_tree () {
    # Update the index
    git update-index -q --ignore-submodules --refresh
    err=0

    # Disallow unstaged changes in the working tree
    if ! git diff-files --quiet --ignore-submodules --
    then
        echo &gt;&amp;2 "cannot $1: you have unstaged changes."
        git diff-files --name-status -r --ignore-submodules -- &gt;&amp;2
        err=1
    fi

    # Disallow uncommitted changes in the index
    if ! git diff-index --cached --quiet HEAD --ignore-submodules --
    then
        echo &gt;&amp;2 "cannot $1: your index contains uncommitted changes."
        git diff-index --cached --name-status -r --ignore-submodules HEAD -- &gt;&amp;2
        err=1
    fi

    if [ $err = 1 ]
    then
        echo &gt;&amp;2 "Please commit or stash them."
        exit 1
    fi
}
*/

/*
https://stackoverflow.com/questions/12641469/list-submodules-in-a-git-repository

ZIE TEST van Peter Mortenseen en Ente Oct 28 '19 18:07u
*/
