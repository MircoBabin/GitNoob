using GitNoob.Gui.Visualizer;
using System.Collections.Generic;

namespace GitNoob.Gui.Program.Remedy
{
    public class MessageSetRemoteUrlFailed : Remedy
    {
        public MessageSetRemoteUrlFailed(Step.Step Step, VisualizerMessageWithLinks Message, 
            string RemoteName, string RemoteUrl) :
            base(Step, ref Message)
        {
            VisualizerMessageText.Append("Error creating/changing remote \"" + RemoteName + "\" url to \"" + RemoteUrl + "\"");

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
