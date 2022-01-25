using System;
using System.Collections.Generic;

namespace GitNoob.Gui.Program.Action.Remedy
{
    public class MessageDetachedHead : Remedy
    {
        public MessageDetachedHead(Step.Step Step, MessageWithLinks Message) :
            base(Step, ref Message)
        {
            VisualizerMessageText.Append("There is no current branch, git is in a detached HEAD state.");
            VisualizerMessageText.Append(Environment.NewLine);
            VisualizerMessageText.Append("Change branch to an existing branch.");

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
