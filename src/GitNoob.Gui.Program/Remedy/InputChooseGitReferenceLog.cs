using GitNoob.Gui.Visualizer;
using GitNoob.Utils;
using System;
using System.Collections.Generic;

namespace GitNoob.Gui.Program.Remedy
{
    public class InputChooseGitReferenceLog : Remedy
    {
        public InputChooseGitReferenceLog(Step.Step Step, VisualizerMessageWithLinks Message,
            IEnumerable<GitResult.GitReflog> reflogs, 
            string CancelText,
            System.Action<GitResult.GitReflog> OnSelectedReflogAction) :
            base(Step, ref Message)
        {
            VisualizerMessageButtons = new List<VisualizerMessageButton>();
            if (!string.IsNullOrWhiteSpace(CancelText))
            {
                VisualizerMessageButtons.Add(new VisualizerMessageButton(CancelText, (input) => { Cancel(); }));
            }

            foreach (var reflog in reflogs)
            {
                VisualizerMessageSubButton showHistory = new VisualizerMessageSubButton(Utils.Resources.getIcon("gitk"), "History of reference log entry", (input) =>
                {
                    StepsExecutor.StartGitk(new List<string>() { reflog.CommitId, "HEAD", StepsExecutor.Config.Git.MainBranch }, reflog.CommitId);
                });

                string message = GitUtils.DateTimeToHumanString(reflog.CommitTime) + " - " + reflog.Selector + " - " + reflog.Message + Environment.NewLine;
                message += reflog.CommitId;
                if (!string.IsNullOrWhiteSpace(reflog.CommitMessage))
                {
                    message += (Environment.NewLine);
                    message += (reflog.CommitMessage);
                }

                VisualizerMessageButtons.Add(new VisualizerMessageButton(message, (input) => {
                    OnSelectedReflogAction(reflog);
                    Done();
                }, new List<VisualizerMessageSubButton>() { showHistory } ));
            }
        }
    }
}
