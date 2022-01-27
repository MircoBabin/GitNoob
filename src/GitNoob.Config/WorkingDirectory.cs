namespace GitNoob.Config
{
    public class WorkingDirectory
    {
        public IProjectType ProjectType { get; set; }

        public string Path { get; set; }

        public string Name { get; set; }
        public string IconFilename { get; set; }
        public string ImageFilename { get; set; }
        public string ImageBackgroundColor { get; set; } //In html notation e.g. "#000000" for black "#ffffff" for white

        public WorkingGit Git { get; set; }
        public Webpage Webpage { get; set; }
        public Php Php { get; set; }
        public Apache Apache { get; set; }
        public Ngrok Ngrok { get; set; }
        public SmtpServer SmtpServer { get; set; }
        public Editor Editor { get; set; }

        public WorkingDirectory()
        {
            ProjectType = null;

            Git = new WorkingGit();
            Php = new Php();
            Apache = new Apache();
            Ngrok = new Ngrok();
            SmtpServer = new SmtpServer();
            Editor = new Editor();
            Webpage = new Webpage();
        }
    }
}
