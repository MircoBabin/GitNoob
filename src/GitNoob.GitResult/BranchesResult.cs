using System.Collections.Generic;

namespace GitNoob.GitResult
{
    public class BranchesResult
    {
        public bool WorkingTreeChanges { get; set; }
        public bool StagedUncommittedFiles { get; set; }
        public bool DetachedHead_NotOnBranch { get; set; }
        public string CurrentBranch { get; set; }
        public bool CurrentBranchIsTrackingRemoteBranch { get; set; }
        public List<string> Branches { get; set; }

        public BranchesResult()
        {
            WorkingTreeChanges = false;
            StagedUncommittedFiles = false;
            DetachedHead_NotOnBranch = false;
            CurrentBranch = string.Empty;
            CurrentBranchIsTrackingRemoteBranch = false;
            Branches = new List<string>();
        }
    }
}
