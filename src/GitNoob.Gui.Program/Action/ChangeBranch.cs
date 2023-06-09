using System.Collections.Generic;
using System.Drawing;

namespace GitNoob.Gui.Program.Action
{
    public class ChangeBranch : Action
    {
        public ChangeBranch(ProgramWorkingDirectory Config) : base(Config) { }

        public override bool isStartable()
        {
            return true;
        }

        public override Icon icon()
        {
            return null;
        }

        public override void execute()
        {
            if (config.Visualizer.isFrontendLocked()) return;

            var executor = new StepsExecutor.StepsExecutor(config, new List<StepsExecutor.IExecutableByStepsExecutor>()
            {
                new Step.EnsureStatus("Change branch is not possible.", false, Step.EnsureStatus.WorkingTreeChanges.FalseAndCanTemporaryCommit, false, false, null),
                new Step.AskChangeBranch()
            });
            executor.execute();
        }
    }
}
