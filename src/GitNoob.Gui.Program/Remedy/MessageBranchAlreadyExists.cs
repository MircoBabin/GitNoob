using GitNoob.Gui.Visualizer;
using System.Collections.Generic;

namespace GitNoob.Gui.Program.Remedy
{
    public class MessageBranchAlreadyExists : Remedy
    {
        public MessageBranchAlreadyExists(Step.Step Step, VisualizerMessageWithLinks Message, string BranchName) :
            base(Step, ref Message)
        {
            VisualizerMessageText.Append("The branch \"" + BranchName + "\" already exists.");

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
