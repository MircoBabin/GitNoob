using System;
using System.Threading;

namespace GitNoob.Gui.Forms
{
    public class WorkingDirectoryRefreshStatus : IDisposable
    {
        private Program.ProgramWorkingDirectory _config;
        System.Action<Git.Result.StatusResult> _onStatusRefreshed;
        System.Action<Exception> _onStatusRefreshedError;

        private volatile bool _refreshAbort = false;
        private volatile bool _refreshNow = true;
        private volatile bool _refreshSuspended = false;
        private Thread _refreshThread;

        public WorkingDirectoryRefreshStatus(Program.ProgramWorkingDirectory Config, 
            System.Action<Git.Result.StatusResult> OnStatusRefreshed,
            System.Action<Exception> OnStatusRefreshedError) 
        {
            _config = Config;
            _onStatusRefreshed = OnStatusRefreshed;
            _onStatusRefreshedError = OnStatusRefreshedError;

            _refreshThread = new Thread(RefreshThreadMain);
            _refreshThread.Name = "RefreshThread - " + Config.Project.Name;
            _refreshThread.Start();
        }

        public void Suspend()
        {
            _refreshSuspended = true;
        }

        public void Resume()
        {
            _refreshNow = true;
            _refreshSuspended = false;
        }

        public void RefreshNow()
        {
            _refreshNow = true;
        }

        #region IDisposable
        public void Dispose()
        {
            Dispose(true);
        }

        ~WorkingDirectoryRefreshStatus()
        {
            Dispose(false);
        }

        protected bool _isDisposed = false;
        protected void Dispose(bool disposing)
        {
            if (_isDisposed) return;
            _isDisposed = true;

            _refreshAbort = true;
        }
        #endregion

        private void RefreshThreadMain()
        {
            DateTime last = new DateTime(1975, 9, 12);
            while (!_refreshAbort)
            {
                if ((!_refreshSuspended) && (_refreshNow || (DateTime.Now - last).TotalSeconds >= 60))
                {
                    _refreshNow = false;

                    try
                    {
                        Git.Result.StatusResult status = _config.Git.RetrieveStatus();

                        try
                        {
                            _onStatusRefreshed(status);
                        }
                        catch { }
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            _onStatusRefreshedError(ex);
                        }
                        catch { }
                    }

                    last = DateTime.Now;
                }

                if (!_refreshAbort)
                {
                    Thread.Sleep(1000);
                }
            }
        }
    }
}
