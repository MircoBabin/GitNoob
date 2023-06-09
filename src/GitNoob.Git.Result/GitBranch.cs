namespace GitNoob.Git.Result
{
    public class GitBranch
    {
        public enum BranchType { Local, LocalTrackingRemoteBranch, UntrackedRemoteBranch }

        public string FullName { get; set; }
        public string ShortName { get; set; }
        public BranchType Type { get; set; }
        public string RemoteBranchFullName { get; set; }
        public string RemoteBranchShortName { get; set; }

        public GitBranch(string FullName, string ShortName, BranchType Type, 
                         string RemoteBranchFullName = null, string RemoteBranchShortName = null)
        {
            this.FullName = FullName;
            this.ShortName = ShortName;
            this.Type = Type;
            this.RemoteBranchFullName = RemoteBranchFullName;
            this.RemoteBranchShortName = RemoteBranchShortName;
        }

        public static string FullnameToShortname(string Fullname)
        {
            int p = Fullname.LastIndexOf('/');
            if (p >= 0)
            {
                return Fullname.Substring(p + 1);
            }

            return Fullname;
        }
    }
}
