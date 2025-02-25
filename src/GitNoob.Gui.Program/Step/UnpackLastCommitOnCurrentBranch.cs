﻿using GitNoob.Gui.Visualizer;

namespace GitNoob.Gui.Program.Step
{
    public class UnpackLastCommitOnCurrentBranch : Step
    {
        bool _onlyUnpackTemporaryCommit;

        public UnpackLastCommitOnCurrentBranch(bool OnlyUnpackTemporaryCommit) : base()
        {
            _onlyUnpackTemporaryCommit = OnlyUnpackTemporaryCommit;
        }

        protected override bool run()
        {
            BusyMessage = "Busy - unpack last commit";

            Git.GitWorkingDirectory.UnpackLastCommitType unpackType;
            if (_onlyUnpackTemporaryCommit)
            {
                unpackType = Git.GitWorkingDirectory.UnpackLastCommitType.OnlyUnpackTemporaryCommit;
            }
            else
            {
                unpackType = Git.GitWorkingDirectory.UnpackLastCommitType.All;
            }
            var result = StepsExecutor.Config.Git.UnpackLastCommitOnCurrentBranch(unpackType, "Safety - unpack - before unpacking: <<lastcommit-message>>");

            var message = new VisualizerMessageWithLinks("Unpack last commit failed.");

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

            return true;
        }
    }
}

