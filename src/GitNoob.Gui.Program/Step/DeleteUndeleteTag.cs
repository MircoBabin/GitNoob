using GitNoob.Gui.Visualizer;
using System.Collections.Generic;

namespace GitNoob.Gui.Program.Step
{
    public class DeleteUndeleteTag : Step
    {
        private GitResult.GitDeletedBranch _branch;
        private IEnumerable<GitResult.GitDeletedBranch> _branches;

        public DeleteUndeleteTag(GitResult.GitDeletedBranch branch) : base()
        {
            _branch = branch;
            _branches = null;
        }

        public DeleteUndeleteTag(IEnumerable<GitResult.GitDeletedBranch> branches) : base()
        {
            _branch = null;
            _branches = branches;
        }

        protected override bool run()
        {
            BusyMessage = "Busy - permanently deleting an undelete tag";

            {
                if (_branch != null)
                    StepsExecutor.Config.Git.DeleteTag(_branch);
                else if (_branches != null)
                    StepsExecutor.Config.Git.DeleteTag(_branches);
            }

            return true;
        }
    }
}
