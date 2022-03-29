namespace GitNoob.Config
{
    public class Ngrok
    {
        public ConfigPath NgrokPath { get; private set; }

        public int Port { get; set; }

        public Ngrok()
        {
            NgrokPath = new ConfigPath(null);
            Port = 4040; //default NGROK port
        }

        public void CopyFrom(Ngrok other)
        {
            NgrokPath.CopyFrom(other.NgrokPath);
            Port = other.Port;
        }

        public void useWorkingDirectory(WorkingDirectory WorkingDirectory)
        {
            NgrokPath.useWorkingDirectory(WorkingDirectory);
        }
    }
}
