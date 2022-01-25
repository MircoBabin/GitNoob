using System;
using System.Collections.Generic;

namespace GitNoob.Gui.Program.Action.Remedy
{
    public class MessageMoveFailed : Remedy
    {
        public MessageMoveFailed(Step.Step Step, MessageWithLinks Message, 
            string fromBranch, string toBranch, string currentBranch,
            bool RenameFailed, bool RemoveRemoteFailed) :
            base(Step, ref Message)
        {
            VisualizerMessageText.Append("Moving changes from branch \"" + fromBranch + "\" to \"" + toBranch + "\" failed.");
            VisualizerMessageText.Append(Environment.NewLine);
            VisualizerMessageText.Append("The current branch is now \"" + currentBranch + "\".");
            VisualizerMessageText.Append(Environment.NewLine);
            VisualizerMessageText.Append(Environment.NewLine);
            if (RenameFailed)
            {
                VisualizerMessageText.Append("(Rename failed.)");
            }
            if(RemoveRemoteFailed)
            {
                VisualizerMessageText.Append("(Remove remote after rename failed.)");
            }

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
