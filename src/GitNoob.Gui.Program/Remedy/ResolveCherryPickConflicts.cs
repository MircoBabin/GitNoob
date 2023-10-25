using GitNoob.Gui.Visualizer;
using System;
using System.Collections.Generic;

namespace GitNoob.Gui.Program.Remedy
{
    public class ResolveCherryPickConflicts : Remedy
    {
        public ResolveCherryPickConflicts(Step.Step Step, VisualizerMessageWithLinks Message) :
            base(Step, ref Message)
        {
            VisualizerMessageText.Append("There are cherry pick conflicts. ");

            VisualizerMessageText.Append(Environment.NewLine);
            VisualizerMessageText.Append("In these files conflict markers \"");
            VisualizerMessageText.AppendLink("<<<<<<< (copy to clipboard)", () => {
                StepsExecutor.CopyToClipboard("<<<<<<<");
            });
            VisualizerMessageText.Append("\" contents of current branch \"=======\" contents of cherry picked commit \">>>>>>>\" have been inserted inside the text.");
            VisualizerMessageText.Append(Environment.NewLine);
            VisualizerMessageText.Append("Manually correct these files and then stage the modified files.");

            VisualizerMessageButtons = 
                new List<VisualizerMessageButton>()
                {
                    new VisualizerMessageButton("Abort the cherry pick.", (input) => {
                        var step = new Step.CherryPickAbort(true);
                        StepsExecutor.InjectSteps(new List<StepsExecutor.IExecutableByStepsExecutor>() { step });

                        Done();

                    }),
                    new VisualizerMessageButton("The conflicts are manually resolved and staged. Continue the cherry pick.", (input) => {
                        var step = new Step.CherryPickContinue();
                        StepsExecutor.InjectSteps(new List<StepsExecutor.IExecutableByStepsExecutor>() { step });

                        Done();
                    }),
                    new VisualizerMessageButton("Start Git Gui.", (input) => {
                        StepsExecutor.StartGitGui();
                    }),
                };

            if (StepsExecutor.WorkspaceIsStartable())
            {
                VisualizerMessageButtons.Add(new VisualizerMessageButton("Start workspace", (input) =>
                {
                    StepsExecutor.StartWorkspace();
                }));
            }
        }
    }
}
