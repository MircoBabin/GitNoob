using System.Collections.Generic;

namespace GitNoob.Config
{
    public interface IProjectType
    {
        IProjectType_Capabilities Capabilities { get; }

        string OverridePhpTempPath(string TempPath, string GIT_ROOT_DIR);
        string OverridePhpLogPath(string LogPath, string GIT_ROOT_DIR);
        IEnumerable<string> GetConfigurationFiles();
        IEnumerable<string> GetLogfilesPaths();

        IProjectType_ActionResult ClearCache(IExecutor executor);
        IProjectType_ActionResult BuildCache(IExecutor executor);
    }
}
