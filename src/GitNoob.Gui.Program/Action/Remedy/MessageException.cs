using System;
using System.Collections.Generic;

namespace GitNoob.Gui.Program.Action.Remedy
{
    public class MessageException : Remedy
    {
        public MessageException(Step.Step Step, VisualizerMessageWithLinks Message, Exception ex) :
            base(Step, ref Message)
        {
            VisualizerMessageText.Append("An exception occurred:");
            VisualizerMessageText.Append(Environment.NewLine);
            VisualizerMessageText.Append(ex.Message);

            VisualizerMessageButtons = 
                new List<VisualizerMessageButton>()
                {
                    new VisualizerMessageButton("Cancel", (input) => {
                        Cancel();
                    }),
                    new VisualizerMessageButton("Copy details to Windows clipboard", (input) => {
                        StepsExecutor.CopyToClipboard(ex.ToString());
                    }),
                };
        }
    }
}
