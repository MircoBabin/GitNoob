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
                string message = FormatUtils.DateTimeToString(branch.DeletionTime) + " - \"" + branch.BranchName + "\" with main branch \"" + branch.MainBranchName + "\"";
                VisualizerMessageButtons.Add(new VisualizerMessageButton(message, (input) => {
                    OnSelectedBranchAction(branch);
                    Done();
                }));
            }
        }
    }
}
