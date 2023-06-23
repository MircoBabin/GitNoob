using GitNoob.Gui.Visualizer;
using System;
using System.Collections.Generic;

namespace GitNoob.Gui.Program.Remedy
{
    public class MessageConfirmTouchTimestampsOnCurrentBranch : Remedy
    {
        public MessageConfirmTouchTimestampsOnCurrentBranch(Step.Step Step, VisualizerMessageWithLinks Message, string currentBranch) :
            base(Step, ref Message)
        {
            VisualizerMessageText.Append("Set the commit timestamps of all unmerged commits on the current branch \"" + currentBranch + "\"." + Environment.NewLine);
            VisualizerMessageText.Append(Environment.NewLine);
            VisualizerMessageText.Append("This will recreate all unmerged commits. The original commits are saved and can be restored by repair option: undelete a deleted branch.");
            VisualizerMessageButtons = 
                new List<VisualizerMessageButton>()
                {
                    new VisualizerMessageButton("Cancel", (input) => {
                        Cancel();
                    }),
                    new VisualizerMessageButton("Set commit timestamps to the current system time (now).", (input) => {
                        var step = new Step.TouchTimestampsOnCurrentBranch(DateTime.Now);
                        StepsExecutor.InjectSteps(new List<StepsExecutor.IExecutableByStepsExecutor>() { step });
                        Done();
                    }),
                };
        }
    }
}
