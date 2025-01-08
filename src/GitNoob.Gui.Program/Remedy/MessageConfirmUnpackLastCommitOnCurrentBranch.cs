using GitNoob.Gui.Visualizer;
using System;
using System.Collections.Generic;

namespace GitNoob.Gui.Program.Remedy
{
    public class MessageConfirmUnpackLastCommitOnCurrentBranch : Remedy
    {
        public MessageConfirmUnpackLastCommitOnCurrentBranch(Step.Step Step, VisualizerMessageWithLinks Message, string currentBranch, string lastCommitId, string lastCommitMessage) :
            base(Step, ref Message)
        {
            VisualizerMessageText.Append("Unpack last commit on current branch \"" + currentBranch + "\"." + Environment.NewLine);
            VisualizerMessageText.Append(Environment.NewLine);
            VisualizerMessageText.Append(lastCommitId);
            VisualizerMessageText.Append(Environment.NewLine);
            VisualizerMessageText.Append(lastCommitMessage);

            VisualizerMessageButtons = 
                new List<VisualizerMessageButton>()
                {
                    new VisualizerMessageButton("Cancel", (input) => {
                        Cancel();
                    }),
                    new VisualizerMessageButton("Unpack last commit. Put changes of last commit into working directory and remove commit from current branch.", (input) => {
                        var step = new Step.UnpackLastCommitOnCurrentBranch(false);
                        StepsExecutor.InjectSteps(new List<StepsExecutor.IExecutableByStepsExecutor>() { step });
                        Done();
                    }),
                };
        }
    }
}
