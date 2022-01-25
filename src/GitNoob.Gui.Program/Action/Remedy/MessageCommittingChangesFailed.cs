using System.Collections.Generic;

namespace GitNoob.Gui.Program.Action.Remedy
{
    public class MessageCommittingChangesFailed : Remedy
    {
        public MessageCommittingChangesFailed(Step.Step Step, MessageWithLinks Message) :
            base(Step, ref Message)
        {
            VisualizerMessageText.Append("Committing changes failed.");

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
