using GitNoob.Utils;
using System.Collections.Generic;
using System.Drawing;

namespace GitNoob.Gui.Program.Action
{
    public class ClearCache : Action
    {
        public ClearCache(ProgramWorkingDirectory Config) : base(Config) { }

        public override bool isStartable()
        {
            return (
                config.ProjectWorkingDirectory.ProjectType != null && 
                config.ProjectWorkingDirectory.ProjectType.Capabilities.CapableOfClearAndBuildCache
            );
        }

        public override Icon icon()
        {
            return Resources.getIcon("clear cache");
        }

        public override void execute()
        {
            if (config.Visualizer.isFrontendLocked()) return;

            var executor = new StepsExecutor.StepsExecutor(config, new List<StepsExecutor.IExecutableByStepsExecutor>()
            {
                new Step.ClearCache()
            });
            executor.execute();
        }
    }
}
