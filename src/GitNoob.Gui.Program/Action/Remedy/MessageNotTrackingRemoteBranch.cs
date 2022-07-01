using System.Collections.Generic;

namespace GitNoob.Gui.Program.Action.Remedy
{
    public class MessageNotTrackingRemoteBranch : Remedy
    {
        public MessageNotTrackingRemoteBranch(Step.Step Step, VisualizerMessageWithLinks Message, string branchName) :
            base(Step, ref Message)
        {
            VisualizerMessageText.Append("Branch \"" + branchName + "\" is not tracking a remote branch.");

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
