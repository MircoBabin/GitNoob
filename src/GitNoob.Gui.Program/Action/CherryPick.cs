using GitNoob.Utils;
using System.Collections.Generic;
using System.Drawing;

namespace GitNoob.Gui.Program.Action
{
    public class CherryPick : Action
    {
        public CherryPick(ProgramWorkingDirectory Config) : base(Config) { }

        public override bool isStartable()
        {
            return true;
        }

        public override Icon icon()
        {
            return Resources.getIcon("cherry pick");
        }

        public override void execute()
        {
            if (config.Visualizer.isFrontendLocked()) return;

            var executor = new StepsExecutor.StepsExecutor(config, new List<StepsExecutor.IExecutableByStepsExecutor>()
            {
                new Step.EnsureStatus("Cherry pick is not possible.", false, Step.EnsureStatus.WorkingTreeChanges.False, false, false, null),
                new Step.GetLatest(true, Step.GetLatest.AllowMovingUnpushedCommitsFromMainBranchType.Never),
                new Step.AskCherryPickCommitId(),
            });
            executor.execute();
        }
    }
}
