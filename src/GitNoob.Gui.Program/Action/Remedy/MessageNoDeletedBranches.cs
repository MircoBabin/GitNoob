using System.Collections.Generic;

namespace GitNoob.Gui.Program.Action.Remedy
{
    public class MessageNoDeletedBranches : Remedy
    {
        public MessageNoDeletedBranches(Step.Step Step, MessageWithLinks Message) :
            base(Step, ref Message)
        {
            VisualizerMessageText.Append("There are no deleted branches.");

            VisualizerMessageButtons =
                new Dictionary<string, System.Action<MessageInput>>()
                {
                    { "Cancel", (input) => {
                        Cancel();
                    } },
                };
        }
    }
}
