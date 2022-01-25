using System.Collections.Generic;

namespace GitNoob.Config
{
    public interface IConfig
    {
        IEnumerable<Project> GetProjects();
    }
}
