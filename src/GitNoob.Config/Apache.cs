namespace GitNoob.Config
{
    public class Apache
    {
        public ConfigPath ApachePath { get; private set; }

        public int Port { get; set; }
        public ConfigFilename ApacheConfTemplateFilename { get; private set; }

        public ConfigPath WebrootPath { get; private set; }

        public Apache()
        {
            ApachePath = new ConfigPath(null);
            ApacheConfTemplateFilename = new ConfigFilename(null);
            WebrootPath = new ConfigPath(null);
        }

        public void CopyFrom(Apache other)
        {
            ApachePath.CopyFrom(other.ApachePath);
            Port = other.Port;
            ApacheConfTemplateFilename.CopyFrom(other.ApacheConfTemplateFilename);
            WebrootPath.CopyFrom(other.WebrootPath);
        }

        public void useWorkingDirectory(WorkingDirectory WorkingDirectory)
        {
            ApachePath.useWorkingDirectory(WorkingDirectory);
            ApacheConfTemplateFilename.useWorkingDirectory(WorkingDirectory);
            WebrootPath.useWorkingDirectory(WorkingDirectory);
        }
    }
}
