using GitNoob.Gui.Visualizer;
using System;
using System.Collections.Generic;

namespace GitNoob.Gui.Program.Remedy
{
    public class CommitName : Remedy
    {
        public CommitName(Step.Step Step, VisualizerMessageWithLinks Message,
            string currentCommitName, string currentCommitEmail,
            string expectedCommitName, string expectedCommitEmail) :
            base(Step, ref Message)
        {
            if (currentCommitName != expectedCommitName)
            { 
                VisualizerMessageText.Append("The current set commit name \"" + currentCommitName + "\" should be \"" + expectedCommitName + "\".");
                VisualizerMessageText.Append(Environment.NewLine);
            }

            if (currentCommitEmail != expectedCommitEmail)
            {
                VisualizerMessageText.Append("The current set commit email \"" + currentCommitEmail + "\" should be \"" + expectedCommitEmail + "\".");
                VisualizerMessageText.Append(Environment.NewLine);
            }

            VisualizerMessageButtons =
                new List<VisualizerMessageButton>()
                {
                    new VisualizerMessageButton("Cancel", (input) => {
                        Cancel();
                    }),

                    new VisualizerMessageButton("Set commit name to " + expectedCommitName + " <" + expectedCommitEmail + "> and continue", (input) => {
                        var step = new Step.SetCommitter(expectedCommitName, expectedCommitEmail);
                        StepsExecutor.InjectSteps(new List<StepsExecutor.IExecutableByStepsExecutor>() { step });

                        Done();
                    }),
                };
        }
    }
}
