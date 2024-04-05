using GitNoob.Gui.Visualizer;
using System;
using System.Collections.Generic;

namespace GitNoob.Gui.Program.Remedy
{
    public class InputNewBranchName : Remedy
    {
        public InputNewBranchName(Step.Step Step, VisualizerMessageWithLinks Message, 
            string NewButtonText,
            bool AskOnCommitId,
            System.Action<string, string> OnNewButtonAction) :
            base(Step, ref Message)
        {
            VisualizerMessageText.Append("A branch name can only contain A-Z, a-z, 0-9, _ (underscore) and - (dash). And should not start with \"gitlock-\" or \"gitnoob-\".");
            VisualizerMessageText.Append(Environment.NewLine);
            VisualizerMessageText.Append("Enter the name of the new branch:");

            if (AskOnCommitId)
            {
                VisualizerMessageInput2.Append("On which commit-id should the branch be created:");
                VisualizerMessageType = IVisualizerMessageType.input2;
            }
            else
            {
                VisualizerMessageType = IVisualizerMessageType.input;
            }

            VisualizerMessageButtons = 
                new List<VisualizerMessageButton>()
                {
                    new VisualizerMessageButton("Cancel", (input) => {
                        Cancel();
                    }),
                    new VisualizerMessageButton(NewButtonText, (input) => {
                        if (input.inputValue.ToLowerInvariant().StartsWith("gitnoob-")) return;
                        if (input.inputValue.ToLowerInvariant().StartsWith("gitlock-")) return;

                        foreach(var c in input.inputValue)
                        {
                            bool valid =
                               (c >= 'A' && c <= 'Z') ||
                               (c >= 'a' && c <= 'z') ||
                               (c >= '0' && c <= '9') ||
                               (c == '_') ||
                               (c == '-');
                            if (!valid) return;
                        }

                        OnNewButtonAction(input.inputValue, (AskOnCommitId ? input.input2Value : null));
                        Done();
                    }),
                    new VisualizerMessageButton("Show history.", (input) => {
                        StepsExecutor.StartGitk(null, String.Empty);
                    }),
                };
        }
    }
}
