using GitNoob.Gui.Visualizer;
using System;
using System.Collections.Generic;

namespace GitNoob.Gui.Program.Remedy
{
    public class MessageBuildingCacheFailed : Remedy
    {
        public MessageBuildingCacheFailed(Step.Step Step, VisualizerMessageWithLinks Message, Config.IProjectType_ActionResult result) :
            base(Step, ref Message)
        {
            VisualizerMessageText.Append("Building the cache failed.");
            VisualizerMessageText.Append(Environment.NewLine);
            VisualizerMessageText.Append(Environment.NewLine);
            VisualizerMessageText.Append(result.Message);

            VisualizerMessageButtons =
                new List<VisualizerMessageButton>()
                {
                    new VisualizerMessageButton("Cancel", (input) => {
                        Cancel();
                    }),
                };
        }
    }
}
