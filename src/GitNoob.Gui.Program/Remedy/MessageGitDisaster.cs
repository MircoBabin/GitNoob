using GitNoob.Gui.Visualizer;
using System;
using System.Collections.Generic;

namespace GitNoob.Gui.Program.Remedy
{
    public class MessageGitDisaster : Remedy
    {
        public MessageGitDisaster(Step.Step Step, VisualizerMessageWithLinks Message, GitResult.BaseGitDisasterResult result) :
            base(Step, ref Message)
        {
            VisualizerMessageButtons =
                new List<VisualizerMessageButton>()
                {
                    new VisualizerMessageButton("Cancel", (input) => {
                        Cancel();
                    }),
                };

            if (result.GitDisaster_RebaseInProgress != false)
            {
                if (result.GitDisaster_MergeInProgress != false)
                    VisualizerMessageText.Append("A rebase/merge is in progress.");
                else
                    VisualizerMessageText.Append("A rebase is in progress.");
                return;
            }

            if (result.GitDisaster_MergeInProgress != false)
            {
                VisualizerMessageText.Append("A merge is in progress.");
                return;
            }

            if (result.GitDisaster_CherryPickInProgress != false)
            {
                VisualizerMessageText.Append("A cherry-pick is in progress.");
                return;
            }

            if (result.GitDisaster_RevertInProgress != false)
            {
                VisualizerMessageText.Append("A revert is in progress.");
                return;
            }

            if (result.GitDisaster_DetachedHead != false)
            {
                VisualizerMessageText.Append("There is no current branch, git is in a detached HEAD state.");
                VisualizerMessageText.Append(Environment.NewLine);
                VisualizerMessageText.Append("Change branch to an existing branch.");
                return;
            }

            if (result.GitDisaster_WorkingTreeChanges != false || result.GitDisaster_StagedUncommittedFiles != false)
            {
                VisualizerMessageText.Append("There are ");
                if (result.GitDisaster_WorkingTreeChanges != false)
                {
                    if (result.GitDisaster_StagedUncommittedFiles != false)
                        VisualizerMessageText.Append("working tree changes and staged uncommitted files.");
                    else
                        VisualizerMessageText.Append("working tree changes.");
                }
                else
                {
                    VisualizerMessageText.Append("staged uncommitted files.");
                }
                return;
            }

            if (result.GitDisaster_UnpushedCommitsOnMainBranch != false)
            {
                // MoveChangesOnMainBranchToNewBranch
                VisualizerMessageText.Append("The main branch \"" + MainBranch + "\" has unpushed commits.");

                VisualizerMessageButtons =
                    new List<VisualizerMessageButton>()
                    {
                    new VisualizerMessageButton("Cancel", (input) => {
                        Cancel();
                    }),
                    new VisualizerMessageButton("Move changes on main branch \"" + MainBranch + "\" to a new branch.", (input) => {
                        var message = new VisualizerMessageWithLinks();
                        message.Append("Move changes on main branch \"" + MainBranch + "\" to a new branch.");

                        var remedy = new InputNewBranchName(Step, message, "Move changes", (NewBranchName) =>
                        {
                            var step = new Step.MoveChangesOnMainBranchToNewBranch(MainBranch, NewBranchName);
                            StepsExecutor.InjectSteps(new List<StepsExecutor.IExecutableByStepsExecutor>() { step });
                        });

                        StepsExecutor.InjectSteps(new List<StepsExecutor.IExecutableByStepsExecutor>() { remedy, Step });
                        Done();
                    }),
                };
                return;
            }

            throw new Exception("(MessageGitDisaster) Unknown disaster");
        }
    }
}
