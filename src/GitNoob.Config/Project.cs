using System.Collections.Generic;

namespace GitNoob.Config
{
    public class Project
    {
        public IProjectType ProjectType { get; set; }
        public ConfigFilename IconFilename { get; private set; }
        public string Name { get; set; }

        public Dictionary<string, WorkingDirectory> WorkingDirectories { get; set; }

        public Project()
        {
            IconFilename = new ConfigFilename(null);
            WorkingDirectories = new Dictionary<string, WorkingDirectory>();
        }
    }
}
