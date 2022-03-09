using System;
using System.Collections.Generic;

namespace GitNoob.Gui.Program.Action.Remedy
{
    public class MessageSetRemoteUrlFailed : Remedy
    {
        public MessageSetRemoteUrlFailed(Step.Step Step, MessageWithLinks Message, 
            string RemoteName, string RemoteUrl) :
            base(Step, ref Message)
        {
            VisualizerMessageText.Append("Error creating/changing remote \"" + RemoteName + "\" url to \"" + RemoteUrl + "\"");

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
