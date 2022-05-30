using System;
using System.Collections.Generic;

namespace GitNoob.Gui.Program.Action.Remedy
{
    public class SelectGitRepairOption : Remedy
    {
        public SelectGitRepairOption(Step.Step Step, MessageWithLinks Message) :
            base(Step, ref Message)
        {
            VisualizerMessageText.Append("Choose a repair option.");

            VisualizerMessageButtons =
                new Dictionary<string, System.Action<MessageInput>>()
                {
                    { "Cancel", (input) => {
                        Cancel();
                    } },
                    { "Show history of all branches / tags / remotes", (input) => {
                        StepsExecutor.StartGitkAll();
                        Done();
                    } },
                    { "Unpack last commit on current branch.", (input) => {
                        var step = new Step.AskUnpackLastCommitOnCurrentBranch();
                        StepsExecutor.InjectSteps(new List<StepsExecutor.IExecutableByStepsExecutor>() { step });
                        Done();
                    } },
                    /*
                    { "Restore a deleted branch.", (input) => {
                        //Todo
                        Done();
                    } },
                    { "Stage all changes and start Git Gui to commit.", (input) => {
                        //Todo
                        Done();
                    } },
                    { "Force main branch to point to a specific commit. And push this change forced to remote.", (input) => {
                        //Todo

                        //also create a local branch on last commit of current main branch, so the commits will not be lost.

                        //var step = new Step.SomethingToBeNamed();
                        //StepsExecutor.InjectSteps(new List<StepsExecutor.IExecutableByStepsExecutor>() { step });

                        Done();
                    } },
                    */
                };
        }
    }
}
