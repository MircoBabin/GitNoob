using GitNoob.Gui.Visualizer;
using GitNoob.Utils;
using System;
using System.Collections.Generic;

namespace GitNoob.Gui.Program.Remedy
{
    public class InputConfirmDeleteUndeleteTag : Remedy
    {
        private bool isSure(string value)
        {
            value = value.Trim().ToLowerInvariant();
            return (value == "sure" || value == "\"sure\"");
        }

        public InputConfirmDeleteUndeleteTag(Step.Step Step, VisualizerMessageWithLinks Message, GitResult.GitDeletedBranch branch) :
            base(Step, ref Message)
        {
            VisualizerMessageText.Append("Permanently delete from the garbage bin:");
            VisualizerMessageText.Append(Environment.NewLine);
            VisualizerMessageText.Append(Environment.NewLine);
            VisualizerMessageText.Append(GitUtils.DateTimeToHumanString(branch.DeletionTime) + " - \"" + branch.BranchName + "\" with main branch \"" + branch.MainBranchName + "\".");
            VisualizerMessageText.Append(Environment.NewLine);
            VisualizerMessageText.Append(branch.Message);

            VisualizerMessageText.Append(Environment.NewLine);
            VisualizerMessageText.Append(Environment.NewLine);
            VisualizerMessageText.Append("Caution: this action is not undoable, the commits will be garabage collected and removed from the git repository at some point!");
            VisualizerMessageText.Append(Environment.NewLine);
            VisualizerMessageText.Append(Environment.NewLine);
            VisualizerMessageText.Append("Confirm this undoable action by typing in \"sure\".");

            VisualizerMessageType = IVisualizerMessageType.input;
            VisualizerMessageButtons =
                new List<VisualizerMessageButton>()
                {
                    new VisualizerMessageButton( "Cancel", (input) => {
                        Cancel();
                    }),
                    new VisualizerMessageButton("Show history and inspect the commits that are about to be lost.", (input) => {
                        StepsExecutor.StartGitk(new List<string>() { branch.Tag.ShortName, branch.MainBranchName, MainBranch }, branch.Tag.PointingToCommitId);
                    }),
                    new VisualizerMessageButton("Permanently delete the commits, irreversible." + Environment.NewLine +
                        "I'm sure and have typed in \"sure\"." + Environment.NewLine, (input) => {
                        if (!isSure(input.inputValue)) return;

                        StepsExecutor.InjectSteps(new List<StepsExecutor.IExecutableByStepsExecutor>() {
                            new Step.DeleteUndeleteTag(branch),
                        });
                        Done();
                    }),
                };
        }
    }
}
