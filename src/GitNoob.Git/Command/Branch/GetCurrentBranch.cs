using GitNoob.GitResult;
using System;

namespace GitNoob.Git.Command.Branch
{
    public class GetCurrentBranch : Command
    {
        public bool? DetachedHead { get; private set; }

        public string fullname { get; private set; }
        private string _shortname;
        public GitBranch branch { get; private set; }

        public string shortname
        {
            get
            {
                if (branch == null)
                {
                    if (!String.IsNullOrWhiteSpace(_shortname)) return _shortname;

                    return null;
                }

                return branch.ShortName;
            }
        }

        private ListBranches list;

        public GetCurrentBranch(GitWorkingDirectory gitworkingdirectory) : base(gitworkingdirectory)
        {
            DetachedHead = null;
            branch = null;
            list = null;
            _shortname = null;

            var current = RunGit("branch", "symbolic-ref -q HEAD");
            var currentshort = RunGit("branchshort", "symbolic-ref -q --short HEAD");
            current.WaitFor();
            switch (current.ExitCode)
            {
                case 0:
                    DetachedHead = false;
                    fullname = current.Output.Trim();
                    break;

                case 1:
                    //detached head
                    DetachedHead = true;
                    fullname = String.Empty;
                    break;
            }

            currentshort.WaitFor();
            switch (current.ExitCode)
            {
                case 0:
                    _shortname = currentshort.Output.Trim();
                    break;
            }


            if (!String.IsNullOrWhiteSpace(fullname))
            {
                list = new ListBranches(gitworkingdirectory, false, fullname);
            }
        }

        protected override void RunGitDone()
        {
            if (list != null)
            {
                list.WaitFor();

                if (list.result != null)
                {
                    // After "git init" without any commit, this will retrieve 0 branches.
                    // A branch must point to a commit, and there are no commits, so there can be no branches.

                    if (list.result.Count > 1)
                    {
                        throw new Exception("Could not list branch \"" + fullname + "\"");
                    }

                    if (list.result.Count == 1)
                    {
                        branch = list.result[0];
                    }
                }
            }
        }
    }
}
