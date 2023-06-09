using System;
using System.Collections.Generic;
using System.Drawing;

namespace GitNoob.Gui.Program.Action
{
    public class VisualizerReady : Action
    {
        public VisualizerReady(ProgramWorkingDirectory Config) : base(Config) { }

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
            throw new Exception("VisualizerReady.execute() is not implemented, use execute(status);");
        }

        private bool _executing = false;
        private List<StepsExecutor.IExecutableByStepsExecutor> _steps = null;
        private int _stepsNo = 0;

        public void execute(Git.Result.StatusResult status)
        {
            if (config.Visualizer.isFrontendLocked()) return;

            if (_executing) throw new Exception("already executing");
            _executing = true;

            _steps = new List<StepsExecutor.IExecutableByStepsExecutor>();

            if (status.DirectoryExists && status.IsGitRootDirectory)
            {
                if (status.Rebasing || status.Merging)
                {
                    _steps.Add(new Step.FinishRebaseMerge());
                }

                if (status.MainBranchExists && !status.MainBranchIsTrackingRemoteBranch)
                {
                    _steps.Add(new Step.AskSetRemoteTrackingBranchForMainBranch());
                }

                if (!string.IsNullOrWhiteSpace(config.ProjectWorkingDirectory.Git.CommitName) &&
                    !string.IsNullOrWhiteSpace(config.ProjectWorkingDirectory.Git.CommitEmail))
                {
                    if (status.CommitName != config.ProjectWorkingDirectory.Git.CommitName ||
                        status.CommitEmail != config.ProjectWorkingDirectory.Git.CommitEmail)
                    {
                        _steps.Add(new Step.AskSetCommitter(status));
                    }
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
                config.Visualizer.unlockFrontend();
                return;
            }

            if (_stepsNo == 0)
            {
                config.Visualizer.lockFrontend();
            }

            var executor = new StepsExecutor.StepsExecutor(config, new List<StepsExecutor.IExecutableByStepsExecutor>()
            {
                _steps[_stepsNo++],
            }, 
            StepsExecutor.StepsExecutor.LockFrontend.No, executeStep);
            executor.execute();
        }
    }
}
