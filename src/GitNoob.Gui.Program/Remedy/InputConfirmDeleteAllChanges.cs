using GitNoob.Gui.Visualizer;
using System;
using System.Collections.Generic;
using System.IO;

namespace GitNoob.Gui.Program.Remedy
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
            VisualizerMessageText.Append("Keep all commits.");
            VisualizerMessageText.Append(Environment.NewLine);
            VisualizerMessageText.Append(Environment.NewLine);
            VisualizerMessageText.Append("Caution: this action is not undoable, all uncommitted changed files will be reverted! All uncommitted changes will be lost!");
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
                    new VisualizerMessageButton("Create a temporary commit to store all current changes." + Environment.NewLine +
                      "Then create an new entry in the GitNoob deleted branches. So this action can be undeleted." + Environment.NewLine +
                      "Then remove the temporary commit and with it all current changes from the current branch." + Environment.NewLine +
                      "I'm sure and have typed in \"sure\"." + Environment.NewLine, (input) => {
                        if (!isSure(input.inputValue)) return;

                        StepsExecutor.InjectSteps(new List<StepsExecutor.IExecutableByStepsExecutor>() {
                            new Step.TemporaryCommitChangesOnCurrentBranch(),
                            new Step.CreateUndeletionTagOnCurrentBranch("Safety - delete all changes"),
                            new Step.RemoveLastTemporaryCommitOnCurrentBranch(),
                        });
                        Done();
                    }),
                    new VisualizerMessageButton("Backup current changes, fully copy directory \"" + StepsExecutor.Config.ProjectWorkingDirectory.Path + "\" to \"" + Path.GetFileName(CopyTo) + "\"." + Environment.NewLine + 
                      "Then delete all uncommitted changes." + Environment.NewLine +
                      "I'm sure and have typed in \"sure\"." + Environment.NewLine, (input) => {
                        if (!isSure(input.inputValue)) return;

                        StepsExecutor.InjectSteps(new List<StepsExecutor.IExecutableByStepsExecutor>() { 
                            new Step.CopyDirectory(CopyTo),
                            new Step.DeleteWorkingTreeChangesAndStagedUncommittedFiles()
                        });
                        Done();
                    }),
                    new VisualizerMessageButton("Start Git Gui and inspect the uncommitted changes that are about to be lost.", (input) => {
                        StepsExecutor.StartGitGui();
                    }),
                    new VisualizerMessageButton("Delete all uncommitted changes irreversible." + Environment.NewLine + 
                      "I'm sure and have typed in \"sure\"." + Environment.NewLine +
                      Environment.NewLine +
                      "I have (optionally) created a manual copy of the directory to backup the current changes.", (input) => {
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
