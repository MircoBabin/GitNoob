using System.Collections.Generic;

namespace GitNoob.Config
{
    public interface IProjectType
    {
        IProjectType_Capabilities Capabilities { get; }

        IEnumerable<string> GetConfigurationFiles();
        IEnumerable<string> GetLogfilesPaths();

        IProjectType_ActionResult ClearCache(IExecutor executor);
        IProjectType_ActionResult BuildCache(IExecutor executor);
    }
}
