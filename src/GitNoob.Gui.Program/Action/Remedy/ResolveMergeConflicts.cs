using System;
using System.Collections.Generic;

namespace GitNoob.Gui.Program.Action.Remedy
{
    public class ResolveMergeConflicts : Remedy
    {
        public ResolveMergeConflicts(Step.Step Step, MessageWithLinks Message) :
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
                new Dictionary<string, System.Action<MessageInput>>()
                {
                    { "Abort the merge.", (input) => {
                        var step = new Step.MergeAbort(true);
                        StepsExecutor.InjectSteps(new List<StepsExecutor.IExecutableByStepsExecutor>() { step });

                        Done();

                    } },
                    { "The conflicts are manually resolved and committed. Continue the merge.", (input) => {
                        var step = new Step.MergeContinue();
                        StepsExecutor.InjectSteps(new List<StepsExecutor.IExecutableByStepsExecutor>() { step });

                        Done();
                    } },
                };
        }
    }
}
