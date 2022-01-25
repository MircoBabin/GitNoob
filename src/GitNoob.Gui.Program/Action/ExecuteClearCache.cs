using System.Collections.Generic;
using System.Drawing;

namespace GitNoob.Gui.Program.Action
{
    public class ExecuteClearCache : Action, IAction
    {
        public ExecuteClearCache(StepsExecutor.StepConfig Config) : base(Config) { }

        public bool isStartable()
        {
            return (
                stepConfig.Config.ProjectWorkingDirectory.ProjectType != null && 
                stepConfig.Config.ProjectWorkingDirectory.ProjectType.Capabilities.CapableOfClearAndBuildCache
            );
        }

        public Icon icon()
        {
            return Utils.Resources.getIcon("clear cache");
        }

        public void execute()
        {
            if (stepConfig.Visualizer.isFrontendLocked()) return;

            var executor = new StepsExecutor.StepsExecutor(stepConfig, new List<StepsExecutor.IExecutableByStepsExecutor>()
            {
                new Step.ClearCache()
            });
            executor.execute();
        }
    }
}
