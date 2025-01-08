namespace GitNoob.Config
{
    public class Php
    {
        public ConfigPath Path { get; private set; }
        public ConfigPath TempPath { get; private set; }
        public ConfigPath LogPath { get; private set; }

        public ConfigFilename PhpIniTemplateFilename { get; private set; }

        public Php()
        {
            Path = new ConfigPath(null);
            TempPath = new ConfigPath(null);
            LogPath = new ConfigPath(null);
            PhpIniTemplateFilename = new ConfigFilename(null);
        }

        public void CopyFrom(Php other)
        {
            Path.CopyFrom(other.Path);
            TempPath.CopyFrom(other.TempPath);
            LogPath.CopyFrom(other.LogPath);
            PhpIniTemplateFilename.CopyFrom(other.PhpIniTemplateFilename);
        }

        public void useWorkingDirectory(WorkingDirectory WorkingDirectory)
        {
            Path.useWorkingDirectory(WorkingDirectory);
            TempPath.useWorkingDirectory(WorkingDirectory);
            LogPath.useWorkingDirectory(WorkingDirectory);
            PhpIniTemplateFilename.useWorkingDirectory(WorkingDirectory);
        }
    }
}
