using System.Collections.Generic;

namespace GitNoob.Gui.Program.Action.Remedy
{
    public class MoveChangesOnMainBranchToNewBranch : Remedy
    {
        public MoveChangesOnMainBranchToNewBranch(Step.Step Step, MessageWithLinks Message, string MainBranch) :
            base(Step, ref Message)
        {
            var orgmessage = this.VisualizerMessageText.Message.ToString();
            VisualizerMessageText.Append("The main branch \"" + MainBranch + "\" has unpushed commits.");

            VisualizerMessageButtons =
                new Dictionary<string, System.Action<MessageInput>>()
                {
                    { "Cancel", (input) => {
                        Cancel();
                    } },
                    { "Move changes on main branch \"" + MainBranch + "\" to a new branch.", (input) => {
                        var message = new MessageWithLinks(orgmessage);
                        message.Append("Move changes on main branch \"" + MainBranch + "\" to a new branch.");

                        var remedy = new InputNewBranchName(Step, message, "Move changes", (NewBranchName) =>
                        {
                            var step = new Step.MoveChangesOnMainBranchToNewBranch(MainBranch, NewBranchName);
                            StepsExecutor.InjectSteps(new List<StepsExecutor.IExecutableByStepsExecutor>() { step });
                        });

                        StepsExecutor.InjectSteps(new List<StepsExecutor.IExecutableByStepsExecutor>() { remedy, Step });
                        Done();
                    } },
                };
        }
    }
}
