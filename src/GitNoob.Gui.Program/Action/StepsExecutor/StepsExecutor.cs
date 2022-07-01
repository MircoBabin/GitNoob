using System;
using System.Collections.Generic;
using System.Threading;

namespace GitNoob.Gui.Program.Action.StepsExecutor
{
    public class StepsExecutor
    {
        public delegate void AfterRun(Exception ex);

        private StepConfig _stepConfig;
        public ProgramWorkingDirectory Config { get { return _stepConfig.Config; } }
        public Config.IExecutor Executor { get { return _stepConfig.Executor; } }

        public string CurrentBranchStored { get; set; }
        public Git.GitLock GitLockStored { get; set; }

        private IEnumerable<IExecutableByStepsExecutor> _steps;
        private bool _lockFrontend;
        private AfterRun _onAfterRun;

        public enum LockFrontend { No, Yes }
        public StepsExecutor(StepConfig Config, IEnumerable<IExecutableByStepsExecutor> Steps, LockFrontend LockFrontend = LockFrontend.Yes, AfterRun OnAfterRun = null)
        {
            _stepConfig = Config;
            _steps = Steps;
            _lockFrontend = (LockFrontend == LockFrontend.Yes);
            _onAfterRun = OnAfterRun;

            CurrentBranchStored = null;
            GitLockStored = null;
        }

        private bool _executing = false;
        private Thread _executeThread = null;
        public void execute()
        {
            if (_executing) throw new Exception("already executing");
            _executing = true;
            if (_lockFrontend) _stepConfig.Visualizer.lockFrontend();

            try
            {
                _executeThread = new Thread(run);
                _executeThread.Start();
            }
            catch (Exception ex)
            {
                _executing = false;
                throw ex;
            }
        }

        private bool _executingCancel = false;
        private Object _injectedStepsLock = new Object();
        private List<IExecutableByStepsExecutor> _injectedSteps = new List<IExecutableByStepsExecutor>();
        private void run()
        {
            try
            {
                _executingCancel = false;
                lock (_injectedStepsLock)
                {
                    _injectedSteps.Clear();
                }

                try
                {
                    IExecutableByStepsExecutor step;
                    foreach (var basestep in _steps)
                    {
                        step = basestep;
                        while (step != null)
                        {
                            if (_executingCancel) return;

                            try
                            {
                                step.StepsExecutor = this;
                                bool success = step.execute();

                                if (!success)
                                {
                                    if (step.FailureRemedy == null)
                                    {
                                        Cancel();
                                        return;
                                    }

                                    lock (_injectedStepsLock)
                                    {
                                        _injectedSteps.Insert(0, step.FailureRemedy);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                var remedy = new Remedy.MessageException(null, new VisualizerMessageWithLinks(String.Empty), ex);
                                remedy.StepsExecutor = this;
                                remedy.execute();
                                return;
                            }


                            lock (_injectedStepsLock)
                            {
                                if (_injectedSteps.Count == 0)
                                {
                                    step = null;
                                }
                                else
                                {
                                    step = _injectedSteps[0];
                                    _injectedSteps.RemoveAt(0);
                                }
                            }
                        }
                    }
                }
                finally
                {
                    if (GitLockStored != null)
                    {
                        BusyMessage("Release logical lock.");
                        try
                        {
                            GitLockStored.Release();
                            GitLockStored = null;
                        }
                        catch { }
                    }

                    _executing = false;
                    BusyMessage(null);
                    if (_lockFrontend) _stepConfig.Visualizer.unlockFrontend();
                }
            }
            catch (Exception ex)
            {
                if (_onAfterRun != null)
                {
                    _onAfterRun(ex);
                    return;
                }
                throw (ex);
            }

            if (_onAfterRun != null)
            {
                _onAfterRun(null);
            }
        }

        public void InjectSteps(IEnumerable<IExecutableByStepsExecutor> steps)
        {
            if (!_executing) throw new Exception("not executing");

            lock (_injectedStepsLock)
            {
                _injectedSteps.InsertRange(0, steps);
            }
        }

        public void Cancel()
        {
            if (!_executing) throw new Exception("not executing");
            BusyMessage(null);

            _executingCancel = true;
        }

        public void CopyToClipboard(string text)
        {
            _stepConfig.Visualizer.copyToClipboard(text);
        }

        public void StartExplorer()
        {
            var action = new StartExplorer(Config);
            action.execute();
        }

        public void StartGitCredentialsViaKeePassCommanderOnGithub()
        {
            var action = new StartGitCredentialsViaKeePassCommanderOnGithub();
            action.execute();
        }

        public void StartGitGui()
        {
            var action = new StartGitGui(_stepConfig);
            action.execute();
        }

        public void StartGitkAll()
        {
            var action = new StartGitkAll(_stepConfig);
            action.execute();
        }

        public void BusyMessage(string message)
        {
            _stepConfig.Visualizer.busyMessage(message);
        }

        public void Message(IVisualizerMessage message)
        {
            _stepConfig.Visualizer.message(message);
        }

        public void CurrentBranchChangedTo(string name)
        {
            _stepConfig.Visualizer.notifyCurrentBranchChanged(name);
        }

    }
}
