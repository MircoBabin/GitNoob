namespace GitNoob.Config
{
    public class Apache
    {
        public ConfigPath ApachePath { get; private set; }

        public int Port { get; set; }
        public ConfigFilename ApacheConfTemplateFilename { get; private set; }

        public ConfigPath WebrootPath { get; private set; }

        public ConfigBoolean UseSsl { get; private set; }
        public ConfigFilename SslCertificateKeyFile { get; private set; }
        public ConfigFilename SslCertificateFile { get; private set; }
        public ConfigFilename SslCertificateChainFile { get; private set; }

        public Apache()
        {
            ApachePath = new ConfigPath(null);
            ApacheConfTemplateFilename = new ConfigFilename(null);
            WebrootPath = new ConfigPath(null);

            UseSsl = new ConfigBoolean(false);
            SslCertificateKeyFile = new ConfigFilename(null);
            SslCertificateFile = new ConfigFilename(null);
            SslCertificateChainFile = new ConfigFilename(null);
        }

        public void CopyFrom(Apache other)
        {
            ApachePath.CopyFrom(other.ApachePath);
            Port = other.Port;
            ApacheConfTemplateFilename.CopyFrom(other.ApacheConfTemplateFilename);
            WebrootPath.CopyFrom(other.WebrootPath);

            UseSsl.CopyFrom(other.UseSsl);
            SslCertificateKeyFile.CopyFrom(other.SslCertificateKeyFile);
            SslCertificateFile.CopyFrom(other.SslCertificateFile);
            SslCertificateChainFile.CopyFrom(other.SslCertificateChainFile);
        }

        public void useWorkingDirectory(WorkingDirectory WorkingDirectory)
        {
            ApachePath.useWorkingDirectory(WorkingDirectory);
            ApacheConfTemplateFilename.useWorkingDirectory(WorkingDirectory);
            WebrootPath.useWorkingDirectory(WorkingDirectory);

            UseSsl.useWorkingDirectory(WorkingDirectory);
            SslCertificateKeyFile.useWorkingDirectory(WorkingDirectory);
            SslCertificateFile.useWorkingDirectory(WorkingDirectory);
            SslCertificateChainFile.useWorkingDirectory(WorkingDirectory);
        }
    }
}
