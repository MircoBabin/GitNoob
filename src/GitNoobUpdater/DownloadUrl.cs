namespace GitNoobUpdater
{
    public class DownloadUrl
    {
        public string downloadUrl;
        public GitNoobVersion version;

        public DownloadUrl(string downloadUrl, GitNoobVersion version)
        {
            this.downloadUrl = downloadUrl;
            this.version = version;
        }
    }
}
