using System;
using System.Collections.Generic;

namespace GitNoob.Gui.Program.Action.Remedy
{
    public class MessagePushConflicts : Remedy
    {
        public MessagePushConflicts(Step.Step Step, MessageWithLinks Message, string branchName) :
            base(Step, ref Message)
        {
            VisualizerMessageText.Append("Branch \"" + branchName + "\" could not be updated in the remote repository.");
            VisualizerMessageText.Append(Environment.NewLine);
            VisualizerMessageText.Append("A fellow worker has also merged his/her changes to the remote repository. And has updated one or more files also updated by you. So there are conflicts for you to solve.");
            VisualizerMessageText.Append(Environment.NewLine);
            VisualizerMessageText.Append("Start the merge again.");

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
