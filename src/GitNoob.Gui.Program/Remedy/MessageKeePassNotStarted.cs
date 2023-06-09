using GitNoob.Gui.Visualizer;
using System.Collections.Generic;

namespace GitNoob.Gui.Program.Remedy
{
    public class MessageKeePassNotStarted : Remedy
    {
        public MessageKeePassNotStarted(Step.Step Step, VisualizerMessageWithLinks Message) :
            base(Step, ref Message)
        {
            VisualizerMessageText.AppendLink("Git Credentials via KeePassCommander", () => {
                StepsExecutor.StartGitCredentialsViaKeePassCommanderOnGithub();
            });
            VisualizerMessageText.Append(" is active, but KeePass is not started, is locked or does not contain the credentials.");

            VisualizerMessageButtons =
                new List<VisualizerMessageButton>()
                {
                    new VisualizerMessageButton("Cancel", (input) => {
                        Cancel();
                    }),
                    new VisualizerMessageButton("Retry", (input) => {
                        StepsExecutor.InjectSteps(new List<StepsExecutor.IExecutableByStepsExecutor>() { Step });

                        Done();
                    }),
                };
        }
    }
}
