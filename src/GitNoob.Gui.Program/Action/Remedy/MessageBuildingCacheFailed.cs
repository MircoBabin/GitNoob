using System;
using System.Collections.Generic;

namespace GitNoob.Gui.Program.Action.Remedy
{
    public class MessageBuildingCacheFailed : Remedy
    {
        public MessageBuildingCacheFailed(Step.Step Step, MessageWithLinks Message, Config.IProjectType_ActionResult result) :
            base(Step, ref Message)
        {
            VisualizerMessageText.Append("Building the cache failed.");
            VisualizerMessageText.Append(Environment.NewLine);
            VisualizerMessageText.Append(Environment.NewLine);
            VisualizerMessageText.Append(result.Message);

            VisualizerMessageButtons =
                new Dictionary<string, System.Action<MessageInput>>()
                {
                    { "Cancel", (input) => {
                        Cancel();
                    } }
                };
        }
    }
}
