using System.Collections.Generic;

namespace GitNoob.Gui.Program.Action.Remedy
{
    public class MessageKeePassNotStarted : Remedy
    {
        public MessageKeePassNotStarted(Step.Step Step, MessageWithLinks Message) :
            base(Step, ref Message)
        {
            VisualizerMessageText.AppendLink("Git Credentials via KeePassCommander", () => {
                StepsExecutor.StartGitCredentialsViaKeePassCommanderOnGithub();
            });
            VisualizerMessageText.Append(" is active, but KeePass is not started, is locked or does not contain the credentials.");

            VisualizerMessageButtons =
                new Dictionary<string, System.Action<MessageInput>>()
                {
                    { "Cancel", (input) => {
                        Cancel();
                    } },
                    { "Retry", (input) => {
                        StepsExecutor.InjectSteps(new List<StepsExecutor.IExecutableByStepsExecutor>() { Step });

                        Done();
                    } }
                };
        }
    }
}
