namespace GitNoob.Config
{
    public class WorkingDirectory
    {
        public IProjectType ProjectType { get; set; }

        public ConfigPath Path { get; private set; }

        public string Name { get; set; }
        public ConfigFilename IconFilename { get; set; }
        public ConfigFilename ImageFilename { get; set; }
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

            Path = new ConfigPath(null);

            IconFilename = new ConfigFilename(null);
            ImageFilename = new ConfigFilename(null);

            Git = new WorkingGit();
            Php = new Php();
            Apache = new Apache();
            Ngrok = new Ngrok();
            SmtpServer = new SmtpServer();
            Editor = new Editor();
            Webpage = new Webpage();
        }

        public void useWorkingDirectory()
        {
            Git.useWorkingDirectory(this);
            Php.useWorkingDirectory(this);
            Apache.useWorkingDirectory(this);
            Ngrok.useWorkingDirectory(this);
            SmtpServer.useWorkingDirectory(this);
            Editor.useWorkingDirectory(this);
            Webpage.useWorkingDirectory(this);
        }
    }
}
