using GitNoob.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace GitNoob.Gui.Program.Action
{
    public class Merge : Action
    {
        public Merge(ProgramWorkingDirectory Config) : base(Config) { }

        public override bool isStartable()
        {
            return true;
        }

        public override Icon icon()
        {
            return Resources.getIcon("merge");
        }

        public override void execute()
        {
            if (config.Visualizer.isFrontendLocked()) return;

            var executor = new StepsExecutor.StepsExecutor(config, new List<StepsExecutor.IExecutableByStepsExecutor>()
            {
                new Step.AskStartMerge(),
                new Step.EnsureStatus("Merge is not possible.", false, Step.EnsureStatus.WorkingTreeChanges.False, false, false, null),
                new Step.EnsureMainBranchExistance("Merge is not possible."),
                new Step.CheckGitNoobTemporaryCommitsOnCurrentBranch("Continue merging GitNoob Temporary Commits ?"),
                new Step.LockMainBranch("Merging local changes from " + config.Project.Name + " - " + config.ProjectWorkingDirectory.Name),
                new Step.GetLatest(false, Step.GetLatest.AllowMovingUnpushedCommitsFromMainBranchType.OnlyIfCurrentBranchIsMainBranch),
                new Step.ExecutorStoreCurrentBranch(),

                new Step.TouchTimestampOfCommitsBeforeMerge(),
                new Step.RebaseCurrentBranchOntoMainBranch("Safety - merge - before merging into mainbranch."),
                new Step.MergeCurrentBranchIntoMainBranchFastForwardOnly(),
                new Step.PushMainBranchToRemote(true),

                new Step.RebuildCacheOnMainBranchAndPushToRemote(true),
                new Step.CheckoutBranch(true, String.Empty),
                new Step.RebaseCurrentBranchOntoMainBranch(null),
            });
            executor.execute();
        }
    }
}
