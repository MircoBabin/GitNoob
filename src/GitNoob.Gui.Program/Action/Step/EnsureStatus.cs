namespace GitNoob.Gui.Program.Action.Step
{
    public class EnsureStatus : Step
    {
        public enum WorkingTreeChanges { Null, True, False, FalseAndCanTemporaryCommit}
        private string _message;
        private bool? _detachedHead;
        private WorkingTreeChanges _workingTreeChanges;
        private bool? _stagedUncommittedFiles;
        private bool? _rebasingMerging;
        private bool? _commitName;

        public EnsureStatus(string message, 
            bool? DetachedHead, WorkingTreeChanges WorkingTreeChanges, bool? StagedUncommittedFiles, bool? RebasingMerging,
            bool? CommitName) : base()
        {
            _message = message;

            _detachedHead = DetachedHead;
            _workingTreeChanges = WorkingTreeChanges;
            _stagedUncommittedFiles = StagedUncommittedFiles;
            _rebasingMerging = RebasingMerging;
            _commitName = CommitName;

            if (DetachedHead == true) throw new System.Exception("EnsureStatus: DetachedHead == true is not supported (message is missing)");
            if (WorkingTreeChanges == WorkingTreeChanges.True) throw new System.Exception("EnsureStatus: WorkingTreeChanges == WorkingTreeChanges.True is not supported (message is missing)");
            if (StagedUncommittedFiles == true) throw new System.Exception("EnsureStatus: StagedUncommittedFiles == true is not supported (message is missing)");
            if (RebasingMerging == true) throw new System.Exception("EnsureStatus: RebasingMerging == true is not supported (message is missing)");
            if (CommitName == false) throw new System.Exception("EnsureStatus: CommitName == false is not supported");
        }

        protected override bool run()
        {
            BusyMessage = "Busy - checking status";

            var result = StepsExecutor.Config.Git.RetrieveStatus();

            var message = new MessageWithLinks(_message);

            if (_rebasingMerging != null)
            {
                if ((result.Rebasing || result.Merging) != _rebasingMerging)
                {
                    FailureRemedy = new Remedy.MessageRebasingOrMerging(this, message, result.Rebasing, result.Merging);
                    return false;
                }
            }

            if (_detachedHead != null)
            {
                if (result.DetachedHead_NotOnBranch != _detachedHead)
                {
                    FailureRemedy = new Remedy.MessageDetachedHead(this, message);
                    return false;
                }
            }

            if (_stagedUncommittedFiles != null)
            {
                if (result.HasStagedUncommittedFiles != _stagedUncommittedFiles)
                {
                    FailureRemedy = new Remedy.MessageChanges(this, message, result.HasWorkingTreeChanges, result.HasStagedUncommittedFiles);
                    return false;
                }
            }

            if (_commitName != null)
            {
                if (!string.IsNullOrWhiteSpace(StepsExecutor.Config.ProjectWorkingDirectory.Git.CommitName) &&
                    !string.IsNullOrWhiteSpace(StepsExecutor.Config.ProjectWorkingDirectory.Git.CommitEmail))
                {
                    if (result.CommitName != StepsExecutor.Config.ProjectWorkingDirectory.Git.CommitName ||
                        result.CommitEmail != StepsExecutor.Config.ProjectWorkingDirectory.Git.CommitEmail)
                    {
                        FailureRemedy = new Remedy.CommitName(this, message, 
                            result.CommitName, result.CommitEmail,
                            StepsExecutor.Config.ProjectWorkingDirectory.Git.CommitName, StepsExecutor.Config.ProjectWorkingDirectory.Git.CommitEmail);
                        return false;
                    }
                }
            }

            switch(_workingTreeChanges)
            {
                case WorkingTreeChanges.Null:
                    break;

                case WorkingTreeChanges.True:
                    if (result.HasWorkingTreeChanges != true)
                    {
                        throw new System.Exception("message is missing");
                    }
                    break;

                case WorkingTreeChanges.False:
                    if (result.HasWorkingTreeChanges != false)
                    {
                        FailureRemedy = new Remedy.MessageChanges(this, message, result.HasWorkingTreeChanges, result.HasStagedUncommittedFiles);
                        return false;
                    }
                    break;

                case WorkingTreeChanges.FalseAndCanTemporaryCommit:
                    if (result.HasWorkingTreeChanges != false)
                    {
                        FailureRemedy = new Remedy.MessageTemporaryCommitWorkingTreeChanges(this, message);
                        return false;
                    }
                    break;

                default:
                    throw new System.Exception("Unknown WorkingTreeChanges value: " + _workingTreeChanges);
            }

            return true;
        }
    }
}
