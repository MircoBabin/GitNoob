using GitNoob.Gui.Visualizer;
using System;
using System.Collections.Generic;

namespace GitNoob.Gui.Program.Remedy
{
    public class MessageDetachedHead : Remedy
    {
        public MessageDetachedHead(Step.Step Step, VisualizerMessageWithLinks Message) :
            base(Step, ref Message)
        {
            VisualizerMessageText.Append("There is no current branch, git is in a detached HEAD state.");
            VisualizerMessageText.Append(Environment.NewLine);
            VisualizerMessageText.Append("Change branch to an existing branch.");

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
