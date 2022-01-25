using System;

namespace GitNoob.Git.Command.Branch
{
    public class GetCurrentBranch : Command
    {
        public bool? DetachedHead { get; private set; }

        public string fullname { get; private set; }
        public GitBranch branch { get; private set; }
        public string shortname
        {
            get
            {
                if (branch == null) return null;

                return branch.ShortName;
            }
        }

        private ListBranches list;

        public GetCurrentBranch(GitWorkingDirectory gitworkingdirectory) : base(gitworkingdirectory)
        {
            DetachedHead = null;
            branch = null;
            list = null;

            var current = RunGit("branch", "symbolic-ref -q HEAD");
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

                if (list.result == null || list.result.Count != 1)
                {
                    throw new Exception("Could not list branch \"" + fullname + "\"");
                }

                branch = list.result[0];
            }
        }
    }
}
