using System.Collections.Generic;

namespace GitNoob.Config
{
    public class Project
    {
        public IProjectType ProjectType { get; set; }
        public string IconFilename { get; set; }
        public string Name { get; set; }

        public Dictionary<string, WorkingDirectory> WorkingDirectories { get; set; }

        public Project()
        {
            WorkingDirectories = new Dictionary<string, WorkingDirectory>();
        }
    }
}
