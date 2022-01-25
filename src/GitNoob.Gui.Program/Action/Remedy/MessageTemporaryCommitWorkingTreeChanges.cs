using System;
using System.Collections.Generic;

namespace GitNoob.Gui.Program.Action.Remedy
{
    public class MessageTemporaryCommitWorkingTreeChanges : Remedy
    {
        public MessageTemporaryCommitWorkingTreeChanges(Step.Step Step, MessageWithLinks Message) :
            base(Step, ref Message)
        {
            VisualizerMessageText.Append("There are working tree changes.");
            VisualizerMessageText.Append(Environment.NewLine);
            VisualizerMessageText.Append(Environment.NewLine);
            VisualizerMessageText.Append("If the last commit on a branch is a temporary commit:");
            VisualizerMessageText.Append(Environment.NewLine);
            VisualizerMessageText.Append("- It will not be merged in the main branch. It will not be pushed to remote.");
            VisualizerMessageText.Append(Environment.NewLine);
            VisualizerMessageText.Append("- When changing to such a branch from within GitNoob, the temporary commit will be unpacked and removed.");

            VisualizerMessageButtons =
                new Dictionary<string, System.Action<MessageInput>>()
                {
                    { "Cancel", (input) => {
                        Cancel();
                    } },
                    { "Create a temporary commit for all working tree changes. So the working tree will be clean. Then retry.", (input) => {
                        var step = new Step.TemporaryCommitChangesOnCurrentBranch();
                        StepsExecutor.InjectSteps(new List<StepsExecutor.IExecutableByStepsExecutor>() { step, Step });

                        Done();
                    } },
                };
        }
    }
}
