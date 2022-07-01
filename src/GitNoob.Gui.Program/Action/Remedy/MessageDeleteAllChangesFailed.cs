using System.Collections.Generic;

namespace GitNoob.Gui.Program.Action.Remedy
{
    public class MessageDeleteAllChangesFailed : Remedy
    {
        public MessageDeleteAllChangesFailed(Step.Step Step, VisualizerMessageWithLinks Message) :
            base(Step, ref Message)
        {
            VisualizerMessageText.Append("There are still working tree changes and/or staged uncommitted files.");

            VisualizerMessageButtons =
                new List<VisualizerMessageButton>()
                {
                    new VisualizerMessageButton("Cancel", (input) => {
                        Cancel();
                    }),
                    new VisualizerMessageButton("Start Git Gui and inspect the changes left.", (input) => {
                        StepsExecutor.StartGitGui();
                        Cancel();
                    }),
                };
        }
    }
}
