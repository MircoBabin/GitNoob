using System.Collections.Generic;

namespace GitNoob.Gui.Program.Action.Remedy
{
    public class MessageChangingBranchFailed : Remedy
    {
        public MessageChangingBranchFailed(Step.Step Step, VisualizerMessageWithLinks Message, string branchName) :
            base(Step, ref Message)
        {
            VisualizerMessageText.Append("Could not change branch to \"" + branchName + "\".");

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
