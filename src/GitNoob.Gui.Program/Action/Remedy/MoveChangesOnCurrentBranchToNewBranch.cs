using System.Collections.Generic;

namespace GitNoob.Gui.Program.Action.Remedy
{
    public class MoveChangesOnCurrentBranchToNewBranch : Remedy
    {
        public MoveChangesOnCurrentBranchToNewBranch(Step.Step Step, MessageWithLinks Message, string CurrentBranch, bool WorkingTreeChanges, bool UnpushedCommits) :
            base(Step, ref Message)
        {
            var orgmessage = this.VisualizerMessageText.Message.ToString();
            VisualizerMessageText.Append("The current branch \"" + CurrentBranch + "\" has ");
            if (WorkingTreeChanges)
            {
                if (UnpushedCommits)
                    VisualizerMessageText.Append("working tree changes and unpushed commits.");
                else
                    VisualizerMessageText.Append("working tree changes.");
            }
            else
            {
                VisualizerMessageText.Append("unpushed commits.");
            }

            VisualizerMessageButtons =
                new Dictionary<string, System.Action<MessageInput>>()
                {
                    { "Cancel", (input) => {
                        Cancel();
                    } },
                    { "Move changes on current branch \"" + CurrentBranch + "\" to a new branch.", (input) => {
                        var message = new MessageWithLinks(orgmessage);
                        message.Append("Move changes on current branch \"" + CurrentBranch + "\" to a new branch.");

                        var remedy = new InputNewBranchName(Step, message, "Move changes", (NewBranchName) =>
                        {
                            var step = new Step.MoveChangesOnCurrentBranchToNewBranch(CurrentBranch, NewBranchName);
                            StepsExecutor.InjectSteps(new List<StepsExecutor.IExecutableByStepsExecutor>() { step });
                        });

                        StepsExecutor.InjectSteps(new List<StepsExecutor.IExecutableByStepsExecutor>() { remedy, Step });
                        Done();
                    } },
                };
        }
    }
}
