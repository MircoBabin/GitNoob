using System;
using System.Collections.Generic;
using System.Drawing;

namespace GitNoob.Gui.Program.Action
{
    public class ExecuteMerge : Action, IAction
    {
        public ExecuteMerge(StepsExecutor.StepConfig Config) : base(Config) { }

        public Icon icon()
        {
            return Utils.Resources.getIcon("merge");
        }

        public void execute()
        {
            if (stepConfig.Visualizer.isFrontendLocked()) return;

            var executor = new StepsExecutor.StepsExecutor(stepConfig, new List<StepsExecutor.IExecutableByStepsExecutor>()
            {
                new Step.AskStartMerge(),
                new Step.EnsureStatus("Merge is not possible.", false, Step.EnsureStatus.WorkingTreeChanges.FalseAndCanTemporaryCommit, false, false, null),
                new Step.EnsureMainBranchExistance("Merge is not possible."),
                new Step.CheckGitNoobTemporaryCommitsOnCurrentBranch("Continue merging GitNoob Temporary Commits ?"),
                new Step.LockMainBranch("Merging local changes from " + stepConfig.Config.Project.Name + " - " + stepConfig.Config.ProjectWorkingDirectory.Name),
                new Step.GetLatest(false),
                new Step.ExecutorStoreCurrentBranch(),

                new Step.RebaseCurrentBranchOntoMainBranch(),
                new Step.MergeCurrentBranchIntoMainBranchFastForwardOnly(),
                new Step.PushMainBranchToRemote(true),

                new Step.RebuildCacheOnMainBranchAndPushToRemote(true),
                new Step.CheckoutBranch(true, String.Empty),
                new Step.RebaseCurrentBranchOntoMainBranch(),
            });
            executor.execute();
        }
    }
}
