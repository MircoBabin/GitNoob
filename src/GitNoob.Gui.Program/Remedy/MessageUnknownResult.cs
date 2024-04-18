using GitNoob.Gui.Visualizer;
using System;
using System.Collections.Generic;

namespace GitNoob.Gui.Program.Remedy
{
    public class MessageUnknownResult : Remedy
    {
        public MessageUnknownResult(Step.Step Step, VisualizerMessageWithLinks Message, Object result) :
            base(Step, ref Message)
        {
            VisualizerMessageText.Append("The result of the operation is unknown.");
            VisualizerMessageText.Append(Environment.NewLine);
            VisualizerMessageText.Append(Environment.NewLine);

            var type = result.GetType();
            VisualizerMessageText.Append(type.Name);
            VisualizerMessageText.Append(Environment.NewLine);

            foreach (var prop in type.GetProperties())
            {
                string value = String.Empty;
                try
                {
                    var valueObject = prop.GetValue(result, null);
                    if (valueObject != null)
                        value = valueObject.ToString();
                    else
                        value = "<NULL>";
                }
                catch (Exception) { }

                VisualizerMessageText.Append(prop.Name);
                VisualizerMessageText.Append(" = ");
                VisualizerMessageText.Append(value);
                VisualizerMessageText.Append(Environment.NewLine);
            }

            VisualizerMessageButtons = 
                new List<VisualizerMessageButton>()
                {
                    new VisualizerMessageButton("Cancel", (input) => {
                        Cancel();
                    }),
                    new VisualizerMessageButton("Copy message to Windows clipboard", (input) => {
                        StepsExecutor.CopyToClipboard(VisualizerMessageText.Message.ToString());
                    }),
                };
        }
    }
}
