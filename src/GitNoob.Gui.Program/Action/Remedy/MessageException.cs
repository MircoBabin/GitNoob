using System;
using System.Collections.Generic;

namespace GitNoob.Gui.Program.Action.Remedy
{
    public class MessageException : Remedy
    {
        public MessageException(Step.Step Step, MessageWithLinks Message, Exception ex) :
            base(Step, ref Message)
        {
            VisualizerMessageText.Append("An exception occurred:");
            VisualizerMessageText.Append(Environment.NewLine);
            VisualizerMessageText.Append(ex.Message);

            VisualizerMessageButtons = 
                new Dictionary<string, System.Action<MessageInput>>()
                {
                    { "Cancel", (input) => {
                        Cancel();
                    } },
                    { "Copy details to Windows clipboard", (input) => {
                        StepsExecutor.CopyToClipboard(ex.ToString());
                    } }
                };
        }
    }
}
