namespace GitNoob.Config
{
    public class Php
    {
        public ConfigPath Path { get; private set; }

        public ConfigFilename PhpIniTemplateFilename { get; private set; }

        public Php()
        {
            Path = new ConfigPath(null);
            PhpIniTemplateFilename = new ConfigFilename(null);
        }

        public void CopyFrom(Php other)
        {
            Path.CopyFrom(other.Path);
            PhpIniTemplateFilename.CopyFrom(other.PhpIniTemplateFilename);
        }

        public void useWorkingDirectory(WorkingDirectory WorkingDirectory)
        {
            Path.useWorkingDirectory(WorkingDirectory);
            PhpIniTemplateFilename.useWorkingDirectory(WorkingDirectory);
        }
    }
}
