using System.Collections.Generic;

namespace GitNoob.GitResult
{
    public class BranchesResult
    {
        public bool DetachedHead_NotOnBranch { get; set; }
        public string CurrentBranch { get; set; }
        public bool CurrentBranchIsTrackingRemoteBranch { get; set; }
        public List<string> Branches { get; set; }

        public BranchesResult()
        {
            DetachedHead_NotOnBranch = false;
            CurrentBranch = string.Empty;
            CurrentBranchIsTrackingRemoteBranch = false;
            Branches = new List<string>();
        }
    }
}
