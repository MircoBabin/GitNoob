using GitNoob.Gui.Visualizer;
using System;
using System.Collections.Generic;

namespace GitNoob.Gui.Program.Remedy
{
    public class ResolveRebaseConflicts : Remedy
    {
        public ResolveRebaseConflicts(Step.Step Step, VisualizerMessageWithLinks Message,
            string MainBranch, string CurrentBranch) :
            base(Step, ref Message)
        {
            VisualizerMessageText.Append("There are rebase conflicts. ");
            if (!String.IsNullOrWhiteSpace(MainBranch) && !String.IsNullOrWhiteSpace(CurrentBranch))
            {
                VisualizerMessageText.Append("The upstream main branch \"");
                VisualizerMessageText.Append(MainBranch);
                VisualizerMessageText.Append("\" has changes in files and the current branch \"");
                VisualizerMessageText.Append(CurrentBranch);
                VisualizerMessageText.Append("\" has commits which changed the same files.");
            }
            else
            {
                VisualizerMessageText.Append("There are files with conflicts.");
            }
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
                    new VisualizerMessageButton("Abort the rebase.", (input) => {
                        var step = new Step.RebaseAbort(true);
                        StepsExecutor.InjectSteps(new List<StepsExecutor.IExecutableByStepsExecutor>() { step });

                        Done();

                    }),
                    new VisualizerMessageButton("The conflicts are manually resolved and committed. Continue the rebase.", (input) => {
                        var step = new Step.RebaseContinue();
                        StepsExecutor.InjectSteps(new List<StepsExecutor.IExecutableByStepsExecutor>() { step });

                        Done();
                    }),
                };
        }
    }
}
