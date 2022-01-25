using System.Collections.Generic;

namespace GitNoob.Gui.Program.Action.Remedy
{
    public class MessageRebasingOrMerging : Remedy
    {
        public MessageRebasingOrMerging(Step.Step Step, MessageWithLinks Message, bool Rebasing, bool Merging) :
            base(Step, ref Message)
        {
            if (Rebasing)
            {
                if (Merging)
                    VisualizerMessageText.Append("A rebase/merge is in progress.");
                else
                    VisualizerMessageText.Append("A rebase is in progress.");
            }
            else
            {
                VisualizerMessageText.Append("A merge is in progress.");
            }

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
