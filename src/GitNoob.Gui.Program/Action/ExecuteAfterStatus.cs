using System;
using System.Collections.Generic;
using System.Drawing;

namespace GitNoob.Gui.Program.Action
{
    public class ExecuteAfterStatus : Action, IAction
    {
        public ExecuteAfterStatus(StepsExecutor.StepConfig Config) : base(Config) { }

        public Icon icon()
        {
            return null;
        }

        public void execute()
        {
            throw new Exception("ExecuteAfterStatus.execute() is not implemented, use execute(status);");
        }

        private bool _executing = false;
        private List<StepsExecutor.IExecutableByStepsExecutor> _steps = null;
        private int _stepsNo = 0;

        public void execute(Git.Result.StatusResult status)
        {
            if (stepConfig.Visualizer.isFrontendLocked()) return;

            if (_executing) throw new Exception("already executing");
            _executing = true;

            _steps = new List<StepsExecutor.IExecutableByStepsExecutor>();

            if (status.Rebasing || status.Merging)
            {
                _steps.Add(new Step.FinishRebaseMerge());
            }

            if (status.MainBranchExists && !status.MainBranchIsTrackingRemoteBranch)
            {
                _steps.Add(new Step.AskSetRemoteTrackingBranchForMainBranch());
            }

            if (!string.IsNullOrWhiteSpace(stepConfig.Config.ProjectWorkingDirectory.Git.CommitName) &&
                !string.IsNullOrWhiteSpace(stepConfig.Config.ProjectWorkingDirectory.Git.CommitEmail))
            {
                if (status.CommitName != stepConfig.Config.ProjectWorkingDirectory.Git.CommitName ||
                    status.CommitEmail != stepConfig.Config.ProjectWorkingDirectory.Git.CommitEmail)
                {
                    _steps.Add(new Step.AskSetCommitter(status));
                }
            }

            if (_steps.Count == 0)
            {
                _executing = false;
                return;
            }

            _stepsNo = 0;
            executeStep(null);
        }

        private void executeStep(Exception ex)
        {
            if (_stepsNo >= _steps.Count)
            {
                _executing = false;
                stepConfig.Visualizer.unlockFrontend();
                return;
            }

            if (_stepsNo == 0)
            {
                stepConfig.Visualizer.lockFrontend();
            }

            var executor = new StepsExecutor.StepsExecutor(stepConfig, new List<StepsExecutor.IExecutableByStepsExecutor>()
            {
                _steps[_stepsNo++],
            }, 
            false, executeStep);
            executor.execute();
        }
    }
}
