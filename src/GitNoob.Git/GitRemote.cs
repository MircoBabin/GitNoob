namespace GitNoob.Git
{
    public class GitRemote
    {
        public string RemoteName { get; set; }
        public string Url { get; set; }

        public GitRemote(string RemoteName, string Url)
        {
            this.RemoteName = RemoteName;
            this.Url = Url;
        }
    }
}
