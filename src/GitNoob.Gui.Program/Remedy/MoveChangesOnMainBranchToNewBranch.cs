using GitNoob.Gui.Visualizer;
using System.Collections.Generic;

namespace GitNoob.Gui.Program.Remedy
{
    public class MoveChangesOnMainBranchToNewBranch : Remedy
    {
        public MoveChangesOnMainBranchToNewBranch(Step.Step Step, VisualizerMessageWithLinks Message, string MainBranch) :
            base(Step, ref Message)
        {
            var orgmessage = this.VisualizerMessageText.Message.ToString();
            VisualizerMessageText.Append("The main branch \"" + MainBranch + "\" has unpushed commits.");

            VisualizerMessageButtons =
                new List<VisualizerMessageButton>()
                {
                    new VisualizerMessageButton("Cancel", (input) => {
                        Cancel();
                    }),
                    new VisualizerMessageButton("Move changes on main branch \"" + MainBranch + "\" to a new branch.", (input) => {
                        var message = new VisualizerMessageWithLinks(orgmessage);
                        message.Append("Move changes on main branch \"" + MainBranch + "\" to a new branch.");

                        var remedy = new InputNewBranchName(Step, message, "Move changes", false, (NewBranchName, OnCommitId) =>
                        {
                            var step = new Step.MoveChangesOnMainBranchToNewBranch(MainBranch, NewBranchName);
                            StepsExecutor.InjectSteps(new List<StepsExecutor.IExecutableByStepsExecutor>() { step });
                        });

                        StepsExecutor.InjectSteps(new List<StepsExecutor.IExecutableByStepsExecutor>() { remedy, Step });
                        Done();
                    }),
                };
        }
    }
}
