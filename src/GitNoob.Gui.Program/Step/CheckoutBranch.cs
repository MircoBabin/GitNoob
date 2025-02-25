﻿using GitNoob.Gui.Visualizer;
using System;

namespace GitNoob.Gui.Program.Step
{
    public class CheckoutBranch : Step
    {
        private bool _useCurrentBranchStored;
        private string _branchName;

        public CheckoutBranch(bool UseCurrentBranchStored, string BranchName) : base()
        {
            if (UseCurrentBranchStored)
            {
                _useCurrentBranchStored = true;
                _branchName = string.Empty;
            }
            else
            {
                _useCurrentBranchStored = false;
                _branchName = BranchName;
            }
        }

        protected override bool run()
        {
            if (_useCurrentBranchStored)
            {
                _branchName = StepsExecutor.CurrentBranchStored;
            }

            BusyMessage = "Busy - changing to branch \"" + _branchName + "\"";

            {
                var result = StepsExecutor.Config.Git.ChangeCurrentBranchTo(_branchName);

                var message = new VisualizerMessageWithLinks("Changing branch failed.");

                if (result.IsGitDisasterHappening != false)
                {
                    FailureRemedy = new Remedy.MessageGitDisaster(this, message, result);
                    return false;
                }

                if (result.Changed)
                {
                    StepsExecutor.CurrentBranchChangedTo(result.CurrentBranch);
                }
                else
                {
                    StepsExecutor.CurrentBranchChangedTo(null);
                }
            }

            {
                var message = new VisualizerMessageWithLinks("Changing branch succeeded.");
                message.Append(Environment.NewLine);
                message.Append(Environment.NewLine);
                message.Append("Optionally unpacking the temporary commit failed.");

                var result = StepsExecutor.Config.Git.UnpackLastCommitOnCurrentBranch(Git.GitWorkingDirectory.UnpackLastCommitType.OnlyUnpackTemporaryCommit, null);
                if (result.IsGitDisasterHappening != false)
                {
                    FailureRemedy = new Remedy.MessageGitDisaster(this, message, result);
                    return false;
                }

                if (!result.NoCommitToUnpack && !result.Unpacked)
                {
                    FailureRemedy = new Remedy.MessageUnknownResult(this, message, result);
                    return false;
                }
            }

            return true;
        }
    }
}
