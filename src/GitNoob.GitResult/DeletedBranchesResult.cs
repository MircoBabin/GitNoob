using System.Collections.Generic;

namespace GitNoob.GitResult
{
    public class DeletedBranchesResult
    {
        public List<GitDeletedBranch> DeletedBranches { get; set; }

        public DeletedBranchesResult()
        {
            DeletedBranches = new List<GitDeletedBranch>();
        }

        public void Sort()
        {
            DeletedBranches.Sort(delegate (GitDeletedBranch x, GitDeletedBranch y)
            {
                //Descending sort on DeletionTime, BranchName
                if (y.DeletionTime < x.DeletionTime) return -1;
                if (y.DeletionTime > x.DeletionTime) return 1;

                return y.BranchName.ToLowerInvariant().CompareTo(x.BranchName.ToLowerInvariant());
            });

        }
    }
}
