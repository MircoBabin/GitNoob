using System.Collections.Generic;

namespace GitNoob.ProjectTypes
{
    public class Php : Config.IProjectType
    {
        private static Config.IProjectType_Capabilities _capabilities = new Config.IProjectType_Capabilities()
        {
            NeedsPhp = true,
            CapableOfClearAndBuildCache = false,
        };
        public Config.IProjectType_Capabilities Capabilities { get { return _capabilities; } }

        public IEnumerable<string> GetConfigurationFiles()
        {
            //Relative to the git root directory.
            return new List<string>();
        }

        public IEnumerable<string> GetLogfilesPaths()
        {
            //Relative to the git root directory.
            return new List<string>();
        }

        public Config.IProjectType_ActionResult ClearCache(Config.IExecutor executor)
        {
            return new Config.IProjectType_ActionResult()
            {
                Result = true,
            };
        }

        public Config.IProjectType_ActionResult BuildCache(Config.IExecutor executor)
        {
            return new Config.IProjectType_ActionResult()
            {
                Result = true,
            };
        }
    }
}
