using System;
using System.Collections.Generic;

namespace GitNoob.Gui.Program.Action.Remedy
{
    public class MessageConfirmStartMerge : Remedy
    {
        public MessageConfirmStartMerge(Step.Step Step, MessageWithLinks Message, string currentBranch, string mainBranch, string remoteUrl) :
            base(Step, ref Message)
        {
            VisualizerMessageText.Append("Merge commits from the current branch \"" + currentBranch + "\" into the main branch \"" + mainBranch + "\". ");
            VisualizerMessageText.Append("And then push the main branch to remote " + remoteUrl + ".");
            VisualizerMessageText.Append(Environment.NewLine);

            VisualizerMessageButtons = 
                new Dictionary<string, System.Action<MessageInput>>()
                {
                    { "Cancel", (input) => {
                        Cancel();
                    } },
                    { "Continue merge", (input) => {
                        Done();
                    } },
                };
        }
    }
}
