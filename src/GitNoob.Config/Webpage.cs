namespace GitNoob.Config
{
    public class Webpage
    {
        public bool Https { get; set; }
        public string Homepage { get; set; }

        public void CopyFrom(Webpage other)
        {
            Https = false;
            Homepage = other.Homepage;
        }

        public void useWorkingDirectory(WorkingDirectory WorkingDirectory)
        {
            Https = WorkingDirectory.Apache.UseSsl.Value;
        }

        public string GetHomepageUrl(int port)
        {
            if (!string.IsNullOrEmpty(Homepage))
                return Homepage.Replace("%port%", port.ToString());

            if (port == 0)
                return null;


            return (Https ? "https" : "http") + "://localhost:" + port + "/";
        }
    }
}
