﻿using GitNoob.Gui.Visualizer;
using System;

namespace GitNoob.Gui.Program.Step
{
    public class LockReset : Step
    {
        private Git.GitLock _lock;

        public LockReset(Git.GitLock gitlock) : base()
        {
            _lock = gitlock;
        }

        protected override bool run()
        {
            BusyMessage = "Busy - resetting logical lock for branch \"" + _lock.branchName + "\"";

            var result = _lock.Reset();

            var message = new VisualizerMessageWithLinks("Resetting Logical lock for branch \"" + _lock.branchName + "\" failed.");

            if (result.IsGitDisasterHappening != false)
            {
                FailureRemedy = new Remedy.MessageGitDisaster(this, message, result);
                return false;
            }

            if (result.ErrorKeePassNotStarted)
            {
                FailureRemedy = new Remedy.MessageKeePassNotStarted(this, message);
                return false;
            }

            if (result.ErrorRemoteNotReachable)
            {
                FailureRemedy = new Remedy.MessageRemoteNotReachable(this, message, StepsExecutor.Config.Git.RemoteUrl);
                return false;
            }

            if (result.ErrorPushingLockTag)
            {
                message.Append(Environment.NewLine);
                message.Append("Error deleting lock tag.");

                FailureRemedy = new Remedy.MessageUnknownResult(this, message, result);
                return false;
            }

            if (!result.Unlocked)
            {
                FailureRemedy = new Remedy.MessageUnknownResult(this, message, result);
                return false;
            }

            if (StepsExecutor.GitLockStored == _lock) StepsExecutor.GitLockStored = null;
            return true;
        }
    }
}
