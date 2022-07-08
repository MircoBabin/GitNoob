using System;
using System.Collections.Generic;

namespace GitNoob.Gui.Program.Action.Remedy
{
    public class InputChooseDeletedBranch : Remedy
    {
        public InputChooseDeletedBranch(Step.Step Step, VisualizerMessageWithLinks Message,
            IEnumerable<Git.GitDeletedBranch> branches, 
            string CancelText,
            string MainBranch,
            System.Action<Git.GitDeletedBranch> OnSelectedBranchAction) :
            base(Step, ref Message)
        {
            VisualizerMessageButtons = new List<VisualizerMessageButton>();
            if (!string.IsNullOrWhiteSpace(CancelText))
            {
                VisualizerMessageButtons.Add(new VisualizerMessageButton(CancelText, (input) => { Cancel(); }));
            }

            foreach (var branch in branches)
            {
                VisualizerMessageSubButton showHistory = new VisualizerMessageSubButton(Utils.Resources.getIcon("gitk"), "Deleted branch history", (input) =>
                {
                    StepsExecutor.StartGitk(new List<string>() { branch.Tag.ShortName, branch.MainBranchName, MainBranch });
                });

                string message = Git.GitUtils.DateTimeToHumanString(branch.DeletionTime) + " - \"" + branch.BranchName + "\" with main branch \"" + branch.MainBranchName + "\"." + Environment.NewLine + branch.Message;
                VisualizerMessageButtons.Add(new VisualizerMessageButton(message, (input) => {
                    OnSelectedBranchAction(branch);
                    Done();
                }, new List<VisualizerMessageSubButton>() { showHistory } ));
            }
        }
    }
}
