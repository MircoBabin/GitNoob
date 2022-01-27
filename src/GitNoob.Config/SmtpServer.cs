namespace GitNoob.Config
{
    public class SmtpServer
    {
        public string Executable { get; set; }

        public SmtpServer()
        {
            Executable = string.Empty;
        }

        public void CopyFrom(SmtpServer other)
        {
            Executable = other.Executable;
        }
    }
}
