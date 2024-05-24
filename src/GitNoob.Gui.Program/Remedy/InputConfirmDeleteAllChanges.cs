using GitNoob.Gui.Visualizer;
using System;
using System.Collections.Generic;

namespace GitNoob.Gui.Program.Remedy
{
    public class InputConfirmDeleteAllChanges : Remedy
    {
        public InputConfirmDeleteAllChanges(Step.Step Step, VisualizerMessageWithLinks Message) :
            base(Step, ref Message)
        {
            VisualizerMessageText.Append("Delete all working tree changes and staged uncommitted files.");

            VisualizerMessageButtons = 
                new List<VisualizerMessageButton>()
                {
                    new VisualizerMessageButton( "Cancel", (input) => {
                        Cancel();
                    }),
                    new VisualizerMessageButton("Create an undelete entry and proceed deleting all changes." + Environment.NewLine, (input) => {
                        StepsExecutor.InjectSteps(new List<StepsExecutor.IExecutableByStepsExecutor>() {
                            new Step.TemporaryCommitChangesOnCurrentBranch(),
                            new Step.CreateUndeletionTagOnCurrentBranch("Safety - delete all changes"),
                            new Step.RemoveLastTemporaryCommitOnCurrentBranch(),
                        });
                        Done();
                    }),
                    new VisualizerMessageButton("Start Git Gui and inspect the uncommitted changes.", (input) => {
                        StepsExecutor.StartGitGui();
                    }),
                };
        }
    }
}
