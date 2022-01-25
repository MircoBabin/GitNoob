using System;
using System.Collections.Generic;

namespace GitNoob.Gui.Program.Action.Remedy
{
    public class FinishRebaseMerge : Remedy
    {
        public FinishRebaseMerge(Step.Step Step, MessageWithLinks Message, bool Rebasing, bool Merging) : 
            base(Step, ref Message)
        {
            if ((Rebasing && Merging) || (!Rebasing && !Merging))
            {
                VisualizerMessageText.Append("A rebase or merge is in progress. Or is killed.");
                VisualizerMessageText.Append(Environment.NewLine);

                VisualizerMessageButtons =
                    new Dictionary<string, System.Action<MessageInput>>()
                    {
                        { "Check if rebase or merge is still in progress.", (input) => {
                            StepsExecutor.InjectSteps(new List<StepsExecutor.IExecutableByStepsExecutor>() { Step });

                            Done();
                        } },
                        { "Abort the rebase.", (input) => {
                            var step = new Step.RebaseAbort(false);
                            StepsExecutor.InjectSteps(new List<StepsExecutor.IExecutableByStepsExecutor>() { step, Step });

                            Done();
                        } },
                        { "Abort the merge.", (input) => {
                            var step = new Step.MergeAbort(false);
                            StepsExecutor.InjectSteps(new List<StepsExecutor.IExecutableByStepsExecutor>() { step, Step });

                            Done();
                        } },
                    };
            }
            else if (Rebasing)
            {
                VisualizerMessageText.Append("A rebase is in progress.");
                VisualizerMessageText.Append(Environment.NewLine);

                VisualizerMessageButtons =
                    new Dictionary<string, System.Action<MessageInput>>()
                    {
                        { "Check if rebase is still in progress.", (input) => {
                            StepsExecutor.InjectSteps(new List<StepsExecutor.IExecutableByStepsExecutor>() { Step });

                            Done();
                        } },
                        { "Abort the rebase.", (input) => {
                            var step = new Step.RebaseAbort(false);
                            StepsExecutor.InjectSteps(new List<StepsExecutor.IExecutableByStepsExecutor>() { step, Step });

                            Done();
                        } },
                    };
            }
            else if (Merging)
            {
                VisualizerMessageText.Append("A merge is in progress.");
                VisualizerMessageText.Append(Environment.NewLine);

                VisualizerMessageButtons =
                    new Dictionary<string, System.Action<MessageInput>>()
                    {
                        { "Check if merge is still in progress.", (input) => {
                            StepsExecutor.InjectSteps(new List<StepsExecutor.IExecutableByStepsExecutor>() { Step });

                            Done();
                        } },
                        { "Abort the merge.", (input) => {
                            var step = new Step.MergeAbort(false);
                            StepsExecutor.InjectSteps(new List<StepsExecutor.IExecutableByStepsExecutor>() { step, Step });

                            Done();
                        } },
                    };
            }

        }
    }
}
