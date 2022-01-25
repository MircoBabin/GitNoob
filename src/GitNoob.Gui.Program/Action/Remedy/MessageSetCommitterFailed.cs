using System;
using System.Collections.Generic;

namespace GitNoob.Gui.Program.Action.Remedy
{
    public class MessageSetCommitterFailed : Remedy
    {
        public MessageSetCommitterFailed(Step.Step Step, MessageWithLinks Message, 
            bool SetNameFailed, bool SetEmailFailed) :
            base(Step, ref Message)
        {
            if (SetNameFailed)
            {
                VisualizerMessageText.Append("Error configuring commit name.");
                VisualizerMessageText.Append(Environment.NewLine);
            }
            if (SetNameFailed)
            {
                VisualizerMessageText.Append("Error configuring commit email.");
                VisualizerMessageText.Append(Environment.NewLine);
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
