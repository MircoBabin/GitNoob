using System.Collections.Generic;
using System.Drawing;

namespace GitNoob.Gui.Program.Action
{
    public class ExecuteDeleteLogfiles : Action, IAction
    {
        public ExecuteDeleteLogfiles(StepsExecutor.StepConfig Config) : base(Config) { }

        public bool isStartable()
        {
            return (StartExploreLogfiles.GetPaths(stepConfig) != null);
        }

        public Icon icon()
        {
            return Utils.Resources.getIcon("delete logfiles");
        }

        public void execute()
        {
            if (!isStartable()) return;

            if (stepConfig.Visualizer.isFrontendLocked()) return;

            var steps = new List<StepsExecutor.IExecutableByStepsExecutor>();
            foreach(var path in StartExploreLogfiles.GetPaths(stepConfig))
            {
                steps.Add(new Step.DeleteLogfiles(path));
            }

            var executor = new StepsExecutor.StepsExecutor(stepConfig, steps);
            executor.execute();
        }
    }
}
