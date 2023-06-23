using GitNoob.Gui.Visualizer;
using GitNoob.Utils;
using System;
using System.Collections.Generic;

namespace GitNoob.Gui.Program.Remedy
{
    public class InputConfirmDeleteAllUndeleteTags : Remedy
    {
        private bool isSure(string value, int count)
        {
            value = value.Trim().ToLowerInvariant();
            return (value == "very sure " + count || value == "\"very sure " + count + "\"");
        }

        public InputConfirmDeleteAllUndeleteTags(Step.Step Step, VisualizerMessageWithLinks Message, IEnumerable<GitResult.GitDeletedBranch> branches) :
            base(Step, ref Message)
        {

            int count = 0;
            foreach (var branch in branches)
            {
                count++;
            }
            VisualizerMessageText.Append("Permanently empty the GitNoob garbage bin.");
            VisualizerMessageText.Append(Environment.NewLine);
            VisualizerMessageText.Append(Environment.NewLine);
            VisualizerMessageText.Append("This will permanently remove all " + count + " undelete entries.");
            VisualizerMessageText.Append(Environment.NewLine);
            VisualizerMessageText.Append("After this action, \"undelete a deleted branch\" will have 0 entries!");
            VisualizerMessageText.Append(Environment.NewLine);

            VisualizerMessageText.Append(Environment.NewLine);
            VisualizerMessageText.Append(Environment.NewLine);
            VisualizerMessageText.Append("Caution: this action is not undoable, the commits will be garabage collected and removed from the git repository at some point!");
            VisualizerMessageText.Append(Environment.NewLine);
            VisualizerMessageText.Append(Environment.NewLine);
            VisualizerMessageText.Append("Confirm this undoable action by typing in \"very sure " + count + "\".");

            VisualizerMessageType = IVisualizerMessageType.input;
            VisualizerMessageButtons =
                new List<VisualizerMessageButton>()
                {
                    new VisualizerMessageButton( "Cancel", (input) => {
                        Cancel();
                    }),
                    new VisualizerMessageButton("Permanently delete all the commits, irreversible." + Environment.NewLine +
                        "I'm sure and have typed in \"very sure " + count + "\"." + Environment.NewLine, (input) => {
                        if (!isSure(input.inputValue, count)) return;

                        StepsExecutor.InjectSteps(new List<StepsExecutor.IExecutableByStepsExecutor>() {
                            new Step.DeleteUndeleteTag(branches),
                        });
                        Done();
                    }),
                };
        }
    }
}
