using GitNoob.Gui.Visualizer;
using GitNoob.Utils;
using System;
using System.Collections.Generic;

namespace GitNoob.Gui.Program.Step
{
    public class AskReferenceLog : Step
    {
        public AskReferenceLog() : base() { }

        protected override bool run()
        {
            BusyMessage = "Busy - retrieving git reference log";

            var result = StepsExecutor.Config.Git.RetrieveGitReferenceLog();

            var message = new VisualizerMessageWithLinks("The git reference log is empty.");

            if (result.GitReferenceLog.Count == 0)
            {
                FailureRemedy = new Remedy.MessageNoGitReferenceLog(this, message);
                return false;
            }

            //not really a failure, but a solution to choose a deleted branch visually
            message = new VisualizerMessageWithLinks("Create new branch:");
            FailureRemedy = new Remedy.InputChooseGitReferenceLog(this, message, result.GitReferenceLog, "Cancel",
                (reflog) => {
                    var msg = new VisualizerMessageWithLinks("Create new branch based on git reference log entry." + Environment.NewLine);
                    msg.Append(GitUtils.DateTimeToHumanString(reflog.CommitTime) + " - " + reflog.Selector + " - " + reflog.Message + Environment.NewLine);
                    msg.Append(reflog.CommitId);
                    if (!string.IsNullOrWhiteSpace(reflog.CommitMessage))
                    {
                        msg.Append(Environment.NewLine);
                        msg.Append(reflog.CommitMessage);
                    }

                    var remedy = new Remedy.InputNewBranchName(this, msg, "Create new branch based on git reference log entry.", false, (input, OnCommitId) => {
                        var createBranch = new CreateBranchOnGitReferenceLog(reflog, input);
                        StepsExecutor.InjectSteps(new List<StepsExecutor.IExecutableByStepsExecutor>() { createBranch });
                    });
                    StepsExecutor.InjectSteps(new List<StepsExecutor.IExecutableByStepsExecutor>() { remedy });
                });
            return false;
        }
    }
}
