namespace GitNoob.Config
{
    public class Webpage
    {
        public string Homepage { get; set; }

        public void CopyFrom(Webpage other)
        {
            Homepage = other.Homepage;
        }

        public string GetHomepageUrl(int port)
        {
            if (!string.IsNullOrEmpty(Homepage))
                return Homepage.Replace("%port%", port.ToString());

            if (port == 0)
                return null;

            return "http://localhost:" + port + "/";
        }
    }
}
