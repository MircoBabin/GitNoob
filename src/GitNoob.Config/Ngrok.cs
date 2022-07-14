namespace GitNoob.Config
{
    public class Ngrok
    {
        public ConfigPath NgrokPath { get; private set; }

        public int Port { get; set; }

        public ConfigFilename AgentConfigurationFile { get; set; }
        public string AuthToken { get; set; }
        public string ApiKey { get; set; }

        public Ngrok()
        {
            NgrokPath = new ConfigPath(null);
            Port = 4040; //default NGROK port
            AgentConfigurationFile = new ConfigFilename(null);
            AuthToken = string.Empty;
            ApiKey = string.Empty;
        }

        public void CopyFrom(Ngrok other)
        {
            NgrokPath.CopyFrom(other.NgrokPath);
            Port = other.Port;

            AgentConfigurationFile.CopyFrom(other.AgentConfigurationFile);
            AuthToken = other.AuthToken;
            ApiKey = other.ApiKey;
        }

        public void useWorkingDirectory(WorkingDirectory WorkingDirectory)
        {
            NgrokPath.useWorkingDirectory(WorkingDirectory);
        }
    }
}
