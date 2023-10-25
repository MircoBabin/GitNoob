using GitNoob.Gui.Visualizer;
using System.Collections.Generic;

namespace GitNoob.Gui.Program.Remedy
{
    public class MessageRebasingOrMerging : Remedy
    {
        public MessageRebasingOrMerging(Step.Step Step, VisualizerMessageWithLinks Message, 
            bool Rebasing, bool Merging) :
            base(Step, ref Message)
        {
            if (Rebasing)
            {
                if (Merging)
                    VisualizerMessageText.Append("A rebase/merge is in progress.");
                else
                    VisualizerMessageText.Append("A rebase is in progress.");
            }
            else
            {
                VisualizerMessageText.Append("A merge is in progress.");
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
