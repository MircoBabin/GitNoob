using GitNoob.Gui.Visualizer;
using System;
using System.Collections.Generic;

namespace GitNoob.Gui.Program.Remedy
{
    public class InputCherryPickCommitId : Remedy
    {
        public InputCherryPickCommitId(Step.Step Step, VisualizerMessageWithLinks Message,
            System.Action<string> OnCherryPickAction) :
            base(Step, ref Message)
        {
            VisualizerMessageText.Append("Which commit-id should be copied (cherry picked) into the current branch?");

            VisualizerMessageType = IVisualizerMessageType.input;
            VisualizerMessageButtons = 
                new List<VisualizerMessageButton>()
                {
                    new VisualizerMessageButton("Cancel", (input) => {
                        Cancel();
                    }),
                    new VisualizerMessageButton("Copy commit-id to current branch", (input) => {
                        OnCherryPickAction(input.inputValue);

                        Done();
                    }),
                };
        }
    }
}
