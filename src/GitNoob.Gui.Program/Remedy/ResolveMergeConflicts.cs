using GitNoob.Gui.Visualizer;
using System;
using System.Collections.Generic;

namespace GitNoob.Gui.Program.Remedy
{
    public class ResolveMergeConflicts : Remedy
    {
        public ResolveMergeConflicts(Step.Step Step, VisualizerMessageWithLinks Message) :
            base(Step, ref Message)
        {
            VisualizerMessageText.Append("There are merge conflicts. ");
            VisualizerMessageText.Append("There are files with conflicts.");

            VisualizerMessageText.Append(Environment.NewLine);
            VisualizerMessageText.Append("In these files conflict markers \"");
            VisualizerMessageText.AppendLink("<<<<<<< (copy to clipboard)", () => {
                StepsExecutor.CopyToClipboard("<<<<<<<");
            });
            VisualizerMessageText.Append("\" contents of upstream main branch \"=======\" contents of current branch \">>>>>>>\" have been inserted inside the text.");
            VisualizerMessageText.Append(Environment.NewLine);
            VisualizerMessageText.Append("Manually correct these files and then commit the final result with a commit message of e.g. \"");
            VisualizerMessageText.AppendLink("Resolved conflicts. (copy to clipboard)", () => {
                StepsExecutor.CopyToClipboard("Resolved conflicts.");
            });
            VisualizerMessageText.Append(".");

            VisualizerMessageButtons = 
                new List<VisualizerMessageButton>()
                {
                    new VisualizerMessageButton("Abort the merge.", (input) => {
                        var step = new Step.MergeAbort(true);
                        StepsExecutor.InjectSteps(new List<StepsExecutor.IExecutableByStepsExecutor>() { step });

                        Done();

                    }),
                    new VisualizerMessageButton("The conflicts are manually resolved and committed. Continue the merge.", (input) => {
                        var step = new Step.MergeContinue();
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
