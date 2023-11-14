using GitNoob.Gui.Visualizer;
using System;
using System.Collections.Generic;

namespace GitNoob.Gui.Program.Remedy
{
    public class MessageNothingCherryPicked : Remedy
    {
        public MessageNothingCherryPicked(Step.Step Step, VisualizerMessageWithLinks Message) :
            base(Step, ref Message)
        {
            VisualizerMessageText.Append("There was nothing cherry picked." + Environment.NewLine);
            VisualizerMessageText.Append("- Was the commit merged ?" + Environment.NewLine);
            VisualizerMessageText.Append("- Is the commit-id correct ?" + Environment.NewLine);

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
