using System;
using System.Collections.Generic;

namespace GitNoob.Gui.Program.Action.Remedy
{
    public class MessageUnexpectedCurrentBranch : Remedy
    {
        public MessageUnexpectedCurrentBranch(Step.Step Step, MessageWithLinks Message, string currentBranch, string expectedBranch) :
            base(Step, ref Message)
        {
            VisualizerMessageText.Append("The current branch \"" + currentBranch + "\" does not meet the expectation.");
            VisualizerMessageText.Append(Environment.NewLine);
            VisualizerMessageText.Append("The current branch is expected to be \"" + expectedBranch + "\".");

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
