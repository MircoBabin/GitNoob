using GitNoob.Gui.Visualizer;
using System;
using System.Collections.Generic;

namespace GitNoob.Gui.Program.Remedy
{
    public class InputConfirmDeleteBranch : Remedy
    {
        public InputConfirmDeleteBranch(Step.Step Step, VisualizerMessageWithLinks Message, string branchName, System.Action<VisualizerInput> OnDelete) :
            base(Step, ref Message)
        {
            VisualizerMessageText.Append("Delete branch \"" + branchName + "\"." + Environment.NewLine);
            VisualizerMessageText.Append(Environment.NewLine);
            VisualizerMessageText.Append("This will remove the branch, but not the commits.The commits can be restored by repair option: undelete a deleted branch.");
            VisualizerMessageText.Append(Environment.NewLine);
            VisualizerMessageText.Append(Environment.NewLine);
            VisualizerMessageText.Append("Enter reason for deletion:");

            VisualizerMessageType = IVisualizerMessageType.input;
            VisualizerMessageButtons = 
                new List<VisualizerMessageButton>()
                {
                    new VisualizerMessageButton("Cancel", (input) => {
                        Cancel();
                    }),
                    new VisualizerMessageButton("Delete branch \"" + branchName + "\".", (input) => {
                        if (string.IsNullOrWhiteSpace(input.inputValue)) return;

                        OnDelete(input);
                        Done();
                    }),
                };
        }
    }
}
