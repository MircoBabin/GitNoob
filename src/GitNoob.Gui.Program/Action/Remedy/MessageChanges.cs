using System.Collections.Generic;

namespace GitNoob.Gui.Program.Action.Remedy
{
    public class MessageChanges : Remedy
    {
        public MessageChanges(Step.Step Step, VisualizerMessageWithLinks Message, bool WorkingTreeChanges, bool StagedUncommittedFiles) :
            base(Step, ref Message)
        {
            VisualizerMessageText.Append("There are ");
            if (WorkingTreeChanges)
            {
                if (StagedUncommittedFiles)
                    VisualizerMessageText.Append("working tree changes and staged uncommitted files.");
                else
                    VisualizerMessageText.Append("working tree changes.");
            }
            else
            {
                VisualizerMessageText.Append("staged uncommitted files.");
            }

            VisualizerMessageButtons =
                new List<VisualizerMessageButton>()
                {
                    new VisualizerMessageButton("Cancel", (input) => {
                        Cancel();
                    }),
                };
        }
    }
}
