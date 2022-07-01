using System.Collections.Generic;

namespace GitNoob.Gui.Program.Action.Remedy
{
    public class MessageClearCacheFailed : Remedy
    {
        public MessageClearCacheFailed(Step.Step Step, VisualizerMessageWithLinks Message, Config.IProjectType_ActionResult result) :
            base(Step, ref Message)
        {
            VisualizerMessageText.Append(result.Message);

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
