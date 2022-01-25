namespace GitNoob.Config
{
    public class Apache
    {
        public string ApachePath { get; set; }

        public int Port { get; set; }
        public string ApacheConfTemplateContents { get; set; }

        public string WebrootPath { get; set; }

        public void CopyFrom(Apache other)
        {
            ApachePath = other.ApachePath;
            Port = other.Port;
            ApacheConfTemplateContents = other.ApacheConfTemplateContents;
            WebrootPath = other.WebrootPath;
        }
    }
}
