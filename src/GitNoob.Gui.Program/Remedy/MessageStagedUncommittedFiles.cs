using GitNoob.Gui.Visualizer;
using System.Collections.Generic;

namespace GitNoob.Gui.Program.Remedy
{
    public class MessageStagedUncommittedFiles : Remedy
    {
        public MessageStagedUncommittedFiles(Step.Step Step, VisualizerMessageWithLinks Message) :
            base(Step, ref Message)
        {
            VisualizerMessageText.Append("There are staged, uncommitted files in the git staging area.");

            VisualizerMessageButtons =
                new List<VisualizerMessageButton>()
                {
                    new VisualizerMessageButton("Cancel", (input) => {
                        Cancel();
                    }),
                    new VisualizerMessageButton("Start Git Gui and manually commit or unstage the staged files", (input) => {
                        StepsExecutor.StartGitGui();
                        Cancel();
                    }),
                };
        }
    }
}
