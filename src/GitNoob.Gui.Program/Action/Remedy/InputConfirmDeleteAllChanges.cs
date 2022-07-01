using System;
using System.Collections.Generic;
using System.IO;

namespace GitNoob.Gui.Program.Action.Remedy
{
    public class InputConfirmDeleteAllChanges : Remedy
    {
        private bool isSure(string value)
        {
            value = value.Trim().ToLowerInvariant();
            return (value == "sure" || value == "\"sure\"");
        }

        public InputConfirmDeleteAllChanges(Step.Step Step, VisualizerMessageWithLinks Message) :
            base(Step, ref Message)
        {
            VisualizerMessageText.Append("Delete all working tree changes and staged uncommitted files.");
            VisualizerMessageText.Append(Environment.NewLine);
            VisualizerMessageText.Append(Environment.NewLine);
            VisualizerMessageText.Append("Caution: this action is not undoable, all changed files will be reverted! All changes will be lost!");
            VisualizerMessageText.Append(Environment.NewLine);
            VisualizerMessageText.Append(Environment.NewLine);
            VisualizerMessageText.Append("Confirm this undoable action by typing in \"sure\".");

            string CopyTo = Utils.FileUtils.DirectoryCopyRenameToDestinationName(StepsExecutor.Config.ProjectWorkingDirectory.Path.ToString());

            VisualizerMessageType = IVisualizerMessageType.input;
            VisualizerMessageButtons = 
                new List<VisualizerMessageButton>()
                {
                    new VisualizerMessageButton( "Cancel", (input) => {
                        Cancel();
                    }),
                    new VisualizerMessageButton("Backup current changes, fully copy directory \"" + StepsExecutor.Config.ProjectWorkingDirectory.Path + "\" to \"" + Path.GetFileName(CopyTo) + "\"." + Environment.NewLine + 
                      "Then delete all changes." + Environment.NewLine +
                      "I'm sure and have typed in \"sure\"." + Environment.NewLine, (input) => {
                        if (!isSure(input.inputValue)) return;

                        StepsExecutor.InjectSteps(new List<StepsExecutor.IExecutableByStepsExecutor>() { 
                            new Step.CopyDirectory(CopyTo),
                            new Step.DeleteWorkingTreeChangesAndStagedUncommittedFiles()
                        });
                        Done();
                    }),
                    new VisualizerMessageButton("Start Git Gui and inspect the changes that are about to be lost.", (input) => {
                        StepsExecutor.StartGitGui();
                    }),
                    new VisualizerMessageButton("Delete all changes irreversible." + Environment.NewLine + 
                      "I'm sure and have typed in \"sure\"." + Environment.NewLine +
                      Environment.NewLine +
                      "I have (optionally) created a copy of the directory to backup the current changes.", (input) => {
                        if (!isSure(input.inputValue)) return;

                        StepsExecutor.InjectSteps(new List<StepsExecutor.IExecutableByStepsExecutor>() {
                            new Step.DeleteWorkingTreeChangesAndStagedUncommittedFiles()
                        });
                        Done();
                    }),
                };
        }
    }
}
