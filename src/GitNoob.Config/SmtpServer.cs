namespace GitNoob.Config
{
    public class SmtpServer
    {
        public ConfigFilename Executable { get; private set; }

        public SmtpServer()
        {
            Executable = new ConfigFilename(null);
        }

        public void CopyFrom(SmtpServer other)
        {
            Executable.CopyFrom(other.Executable);
        }

        public void useWorkingDirectory(WorkingDirectory WorkingDirectory)
        {
            Executable.useWorkingDirectory(WorkingDirectory);
        }
    }
}
