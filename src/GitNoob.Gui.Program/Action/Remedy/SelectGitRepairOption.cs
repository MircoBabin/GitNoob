using System;
using System.Collections.Generic;

namespace GitNoob.Gui.Program.Action.Remedy
{
    public class SelectGitRepairOption : Remedy
    {
        public SelectGitRepairOption(Step.Step Step, VisualizerMessageWithLinks Message) :
            base(Step, ref Message)
        {
            VisualizerMessageText.Append("Choose a repair option.");

            VisualizerMessageButtons =
                new List<VisualizerMessageButton>()
                {
                    new VisualizerMessageButton("Cancel", (input) => {
                        Cancel();
                    }),
                    new VisualizerMessageButton("Show history of all branches / tags / remotes", (input) => {
                        StepsExecutor.StartGitkAll();
                        Done();
                    }),
                    new VisualizerMessageButton("Unpack last commit on current branch.", (input) => {
                        var step = new Step.AskUnpackLastCommitOnCurrentBranch();
                        StepsExecutor.InjectSteps(new List<StepsExecutor.IExecutableByStepsExecutor>() { step });
                        Done();
                    }),
                    new VisualizerMessageButton("Set/touch the author and commit timestamps of all unmerged commits on the current branch.", (input) => {
                        var step = new Step.AskTouchTimestampsOnCurrentBranch();
                        StepsExecutor.InjectSteps(new List<StepsExecutor.IExecutableByStepsExecutor>() { step });
                        Done();
                    }),

                    new VisualizerMessageButton("Undelete a deleted branch.", (input) => {
                        var step = new Step.AskUndeleteBranch();
                        StepsExecutor.InjectSteps(new List<StepsExecutor.IExecutableByStepsExecutor>() { step });
                        Done();
                    }),

                    /*
                    new VisualizerMessageButton("Stage all changes and start Git Gui to commit.", (input) => {
                        //Todo
                        Done();
                    }),

                    new VisualizerMessageButton("Show git reference log.", (input) => {
                        //Todo, with undelete (create branch) option - like undelete a deleted branch.
                        Done();
                    }),

                    new VisualizerMessageButton("Force main branch to point to a specific commit. And push this change forced to remote.", (input) => {
                        //Todo

                        //also create a local branch on last commit of current main branch, so the commits will not be lost.

                        //var step = new Step.SomethingToBeNamed();
                        //StepsExecutor.InjectSteps(new List<StepsExecutor.IExecutableByStepsExecutor>() { step });

                        Done();
                    }),
                    */
                };
        }
    }
}
