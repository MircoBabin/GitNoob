﻿using GitNoob.Gui.Visualizer;

namespace GitNoob.Gui.Program.Step
{
    public class DeleteWorkingTreeChangesAndStagedUncommittedFiles : Step
    {
        public DeleteWorkingTreeChangesAndStagedUncommittedFiles() : base() { }

        protected override bool run()
        {
            BusyMessage = "Busy - deleting all working tree changes and staged uncommitted files.";

            var result = StepsExecutor.Config.Git.DeleteWorkingTreeChangesAndStagedUncommittedFiles();

            var message = new VisualizerMessageWithLinks("Deleting all working tree changes and staged uncommitted files failed.");

            if (result.IsGitDisasterHappening != false)
            {
                FailureRemedy = new Remedy.MessageGitDisaster(this, message, result);
                return false;
            }

            if (result.ErrorStillStagedUncommittedFiles || result.ErrorStillWorkingTreeChanges)
            {
                FailureRemedy = new Remedy.MessageDeleteAllChangesFailed(this, message);
                return false;
            }

            if (!result.ChangesDeleted)
            {
                FailureRemedy = new Remedy.MessageUnknownResult(this, message, result);
                return false;
            }

            return true;
        }
    }
}

