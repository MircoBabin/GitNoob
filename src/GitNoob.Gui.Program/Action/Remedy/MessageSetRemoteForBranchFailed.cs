using System;
using System.Collections.Generic;

namespace GitNoob.Gui.Program.Action.Remedy
{
    public class MessageSetRemoteForBranchFailed : Remedy
    {
        public MessageSetRemoteForBranchFailed(Step.Step Step, MessageWithLinks Message, 
            string BranchName, string RemoteName) :
            base(Step, ref Message)
        {
            VisualizerMessageText.Append("Error setting tracking remote branch to \"" + RemoteName + "/" + BranchName + "\" for branch \"" + BranchName + "\"");

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
