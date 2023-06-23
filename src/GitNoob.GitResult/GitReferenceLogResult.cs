using System.Collections.Generic;

namespace GitNoob.GitResult
{
    public class GitReferenceLogResult
    {
        public List<GitReflog> GitReferenceLog { get; set; }

        public GitReferenceLogResult()
        {
            GitReferenceLog = new List<GitReflog>();
        }

        public void Sort()
        {
            GitReferenceLog.Sort(delegate (GitReflog x, GitReflog y)
            {
                //Descending sort on CommitTime, selector
                if (y.CommitTime < x.CommitTime) return -1;
                if (y.CommitTime > x.CommitTime) return 1;

                return y.Selector.ToLowerInvariant().CompareTo(x.Selector.ToLowerInvariant());
            });

        }
    }
}
