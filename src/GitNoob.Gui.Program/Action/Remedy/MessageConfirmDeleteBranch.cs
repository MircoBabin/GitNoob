using System;
using System.Collections.Generic;

namespace GitNoob.Gui.Program.Action.Remedy
{
    public class MessageConfirmDeleteBranch : Remedy
    {
        public MessageConfirmDeleteBranch(Step.Step Step, VisualizerMessageWithLinks Message, string branchName) :
            base(Step, ref Message)
        {
            VisualizerMessageText.Append("Delete branch \"" + branchName + "\"." + Environment.NewLine);
            VisualizerMessageText.Append(Environment.NewLine);
            VisualizerMessageText.Append("The commits are not deleted. A local tag \"gitnoob-deleted-branch-...\" will be created pointing to the last commit. The commits stay reachable and are not garbage collected.");

            VisualizerMessageButtons = 
                new List<VisualizerMessageButton>()
                {
                    new VisualizerMessageButton("Cancel", (input) => {
                        Cancel();
                    }),
                    new VisualizerMessageButton("Delete branch \"" + branchName + "\".", (input) => {
                        Done();
                    }),
                };
        }
    }
}
