using System.Collections.Generic;
using System.Drawing;

namespace GitNoob.Gui.Program.Action
{
    public class ExecuteGetLatest : Action, IAction
    {
        public ExecuteGetLatest(StepsExecutor.StepConfig Config) : base(Config) { }

        public Icon icon()
        {
            return Utils.Resources.getIcon("get latest");
        }

        public void execute()
        {
            if (stepConfig.Visualizer.isFrontendLocked()) return;

            var executor = new StepsExecutor.StepsExecutor(stepConfig, new List<StepsExecutor.IExecutableByStepsExecutor>()
            {
                new Step.GetLatest(true),
                new Step.EnsureStatus("Get latest was successfull.", null, Step.EnsureStatus.WorkingTreeChanges.Null, null, null, true),
            });
            executor.execute();
        }
    }
}
