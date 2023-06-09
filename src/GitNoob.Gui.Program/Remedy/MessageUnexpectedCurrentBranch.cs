using GitNoob.Gui.Visualizer;
using System;
using System.Collections.Generic;

namespace GitNoob.Gui.Program.Remedy
{
    public class MessageUnexpectedCurrentBranch : Remedy
    {
        public MessageUnexpectedCurrentBranch(Step.Step Step, VisualizerMessageWithLinks Message, string currentBranch, string expectedBranch) :
            base(Step, ref Message)
        {
            VisualizerMessageText.Append("The current branch \"" + currentBranch + "\" does not meet the expectation.");
            VisualizerMessageText.Append(Environment.NewLine);
            VisualizerMessageText.Append("The current branch is expected to be \"" + expectedBranch + "\".");

            VisualizerMessageButtons =
                new List<VisualizerMessageButton>()
                {
                    new VisualizerMessageButton("Cancel", (input) => {
                        Cancel();
                    }),
                };
        }
    }
}
