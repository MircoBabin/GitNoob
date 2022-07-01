using System.Collections.Generic;

namespace GitNoob.Gui.Program.Action.Remedy
{
    public class MessageNoDeletedBranches : Remedy
    {
        public MessageNoDeletedBranches(Step.Step Step, VisualizerMessageWithLinks Message) :
            base(Step, ref Message)
        {
            VisualizerMessageText.Append("There are no deleted branches.");

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
