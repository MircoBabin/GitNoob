using System;
using System.Collections.Generic;

namespace GitNoob.Gui.Program.Action.Remedy
{
    public class MessageConfirmPruneAggressive : Remedy
    {
        public MessageConfirmPruneAggressive(Step.Step Step, VisualizerMessageWithLinks Message) :
            base(Step, ref Message)
        {
            VisualizerMessageText.Append("Empty the git recycle bin?" + Environment.NewLine);
            VisualizerMessageText.Append("- All commits not belonging to a branch/tag are expired immediatly." + Environment.NewLine);
            VisualizerMessageText.Append("- All expired and loose objects are permanently and aggressively removed (pruned)." + Environment.NewLine);
            VisualizerMessageText.Append("- Branches deleted from within GitNoob are not removed, because they are reachable by the GitNoob deletion tag." + Environment.NewLine);
            VisualizerMessageButtons =
                new List<VisualizerMessageButton>()
                {
                    new VisualizerMessageButton("Cancel", (input) => {
                        Cancel();
                    }),
                    new VisualizerMessageButton("Empty the git recycle bin", (input) => {
                        var step = new Step.PruneAggressive();
                        StepsExecutor.InjectSteps(new List<StepsExecutor.IExecutableByStepsExecutor>() { step });
                        Done();
                    }),
                };
        }
    }
}
