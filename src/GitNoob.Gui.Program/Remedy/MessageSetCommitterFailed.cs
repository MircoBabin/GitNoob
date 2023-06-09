using GitNoob.Gui.Visualizer;
using System;
using System.Collections.Generic;

namespace GitNoob.Gui.Program.Remedy
{
    public class MessageSetCommitterFailed : Remedy
    {
        public MessageSetCommitterFailed(Step.Step Step, VisualizerMessageWithLinks Message, 
            bool SetNameFailed, bool SetEmailFailed) :
            base(Step, ref Message)
        {
            if (SetNameFailed)
            {
                VisualizerMessageText.Append("Error configuring commit name.");
                VisualizerMessageText.Append(Environment.NewLine);
            }
            if (SetNameFailed)
            {
                VisualizerMessageText.Append("Error configuring commit email.");
                VisualizerMessageText.Append(Environment.NewLine);
            }

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
