using GitNoob.Gui.Visualizer;
using System.Collections.Generic;

namespace GitNoob.Gui.Program.Remedy
{
    public class MessageNoGitReferenceLog : Remedy
    {
        public MessageNoGitReferenceLog(Step.Step Step, VisualizerMessageWithLinks Message) :
            base(Step, ref Message)
        {
            VisualizerMessageText.Append("There are no entries in the git reference log.");

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
