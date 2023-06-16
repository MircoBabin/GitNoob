using GitNoob.Gui.Visualizer;
using System;
using System.Collections.Generic;

namespace GitNoob.Gui.Program.Step
{
    public class GetLatest : Step
    {
        private bool _rebaseCurrentBranchOntoMainBranchAfterDownload;
        public GetLatest(bool RebaseCurrentBranchOntoMainBranchAfterDownload) : base()
        {
            _rebaseCurrentBranchOntoMainBranchAfterDownload = RebaseCurrentBranchOntoMainBranchAfterDownload;
        }

        protected override bool run()
        {
            BusyMessage = "Busy - getting latest from " + StepsExecutor.Config.ProjectWorkingDirectory.Git.RemoteUrl;

            var result = StepsExecutor.Config.Git.GetLatest();

            var message = new VisualizerMessageWithLinks("Get latest is not possible.");

            if (result.ErrorNonEmptyAndNotAGitRepository)
            {
                message.Append(Environment.NewLine);
                message.Append("The directory \"");
                message.AppendLink(StepsExecutor.Config.ProjectWorkingDirectory.Path.ToString(), () => {
                    StepsExecutor.StartExplorer();
                });
                message.Append("\" is not empty and not a git repository.");

                FailureRemedy = new Remedy.RenameExistingDirectory(this, message);
                return false;
            }

            if (result.ErrorKeePassNotStarted)
            {
                FailureRemedy = new Remedy.MessageKeePassNotStarted(this, message);
                return false;
            }

            if (result.ErrorRemoteNotReachable)
            {
                FailureRemedy = new Remedy.MessageRemoteNotReachable(this, message, StepsExecutor.Config.Git.RemoteUrl);
                return false;
            }

            if (result.ErrorStagedUncommittedFiles)
            {
                FailureRemedy = new Remedy.MessageStagedUncommittedFiles(this, message);
                return false;
            }

            if (result.ErrorWorkingTreeChangesAndOnLocalTrackingRemoteBranch ||
                result.ErrorUnpushedCommitsAndOnLocalTrackingRemoteBranch)
            {
                message.Append(Environment.NewLine);
                message.Append("The current branch is tracking a remote branch.");
                FailureRemedy = new Remedy.MoveChangesOnCurrentBranchToNewBranch(this, message, result.CurrentBranch, result.ErrorWorkingTreeChangesAndOnLocalTrackingRemoteBranch, result.ErrorUnpushedCommitsAndOnLocalTrackingRemoteBranch);
                return false;
            }

            if (result.ErrorUnpushedCommitsOnMainBranch)
            {
                FailureRemedy = new Remedy.MoveChangesOnMainBranchToNewBranch(this, message, MainBranch);
                return false;
            }

            if (!result.Cloned && !result.Updated)
            {
                FailureRemedy = new Remedy.MessageUnknownResult(this, message, result);
                return false;
            }

            //Try to ensure main branch existance
            StepsExecutor.Config.Git.EnsureMainBranchExistance();

            //Success, check status for rebase current branch onto main
            if (!_rebaseCurrentBranchOntoMainBranchAfterDownload)
            {
                return true;
            }

            //not really a failure, but a solution to present a message
            message = new VisualizerMessageWithLinks("Get latest has been successfull.");

            if (!result.DetachedHead_NotOnBranch && result.CurrentBranchIsBehindMainBranch)
            {
                if (result.UnpushedCommits)
                {
                    if (result.WorkingTreeChanges || result.StagedUncommittedFiles)
                    {
                        message.Append(Environment.NewLine);
                        message.Append("The current branch \"" + result.CurrentBranch + "\" is not updated, because there are uncommitted changes. The current branch does not contain the downloaded changes of the main branch \"" + MainBranch + "\"." + Environment.NewLine);
                        message.Append(Environment.NewLine);
                        message.Append("If you want to incorporate the latest main branch changes in the current branch, then commit the current changes as a \"work in progress\" (wip) and retry get latest.");
                        FailureRemedy = new Remedy.MessageChanges(this, message, result.WorkingTreeChanges, result.StagedUncommittedFiles);
                        return false;
                    }

                    FailureRemedy = new Remedy.RebaseCurrentBranchOntoMainBranch(this, message, MainBranch, result.CurrentBranch);
                    return false;
                }

                //current branch has no unpushed commits, no staged uncommitted files and no working tree changes.
                //automatic rebase onto main branch.
                var step = new RebaseCurrentBranchOntoMainBranch(null);
                StepsExecutor.InjectSteps(new List<StepsExecutor.IExecutableByStepsExecutor>() { step });
                return true;
            }

            return true;
        }
    }
}
