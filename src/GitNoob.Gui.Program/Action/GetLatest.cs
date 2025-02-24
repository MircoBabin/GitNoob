using GitNoob.Utils;
using System.Collections.Generic;
using System.Drawing;

namespace GitNoob.Gui.Program.Action
{
    public class GetLatest : Action
    {
        public GetLatest(ProgramWorkingDirectory Config) : base(Config) { }

        public override bool isStartable()
        {
            return true;
        }

        public override Icon icon()
        {
            return Resources.getIcon("get latest");
        }

        public override void execute()
        {
            if (config.Visualizer.isFrontendLocked()) return;

            var executor = new StepsExecutor.StepsExecutor(config, new List<StepsExecutor.IExecutableByStepsExecutor>()
            {
                new Step.GetLatest(true, Step.GetLatest.AllowMovingUnpushedCommitsFromMainBranchType.OnlyIfCurrentBranchIsMainBranch),
                new Step.EnsureStatus("Get latest was successfull.", null, Step.EnsureStatus.WorkingTreeChanges.Null, null, null, true),
            });
            executor.execute();
        }
    }
}
