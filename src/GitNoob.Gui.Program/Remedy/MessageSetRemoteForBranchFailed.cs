using GitNoob.Gui.Visualizer;
using System.Collections.Generic;

namespace GitNoob.Gui.Program.Remedy
{
    public class MessageSetRemoteForBranchFailed : Remedy
    {
        public MessageSetRemoteForBranchFailed(Step.Step Step, VisualizerMessageWithLinks Message, 
            string BranchName, string RemoteName) :
            base(Step, ref Message)
        {
            VisualizerMessageText.Append("Error setting tracking remote branch to \"" + RemoteName + "/" + BranchName + "\" for branch \"" + BranchName + "\"");

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
