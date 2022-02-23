﻿using System;
using System.IO;
using System.Threading;

namespace GitNoob.Gui.Forms
{
    public class WorkingDirectoryRefreshStatus : IDisposable
    {
        private Program.ProgramWorkingDirectory _config;
        System.Action<Git.Result.StatusResult> _onStatusRefreshed;
        System.Action<Exception> _onStatusRefreshedError;

        private Semaphore _refreshAbort = new Semaphore(0, int.MaxValue);
        private volatile bool _refreshNow = true;
        private volatile bool _refreshSuspended = false;
        private Thread _refreshThread;

        private FileSystemWatcher _watcher;

        public WorkingDirectoryRefreshStatus(Program.ProgramWorkingDirectory Config, 
            System.Action<Git.Result.StatusResult> OnStatusRefreshed,
            System.Action<Exception> OnStatusRefreshedError) 
        {
            _config = Config;
            _onStatusRefreshed = OnStatusRefreshed;
            _onStatusRefreshedError = OnStatusRefreshedError;

            _refreshThread = new Thread(RefreshThreadMain);
            _refreshThread.Name = "RefreshThread - " + Config.Project.Name + " - " + Config.ProjectWorkingDirectory.Name;
            _refreshThread.Start();

            _watcher = new FileSystemWatcher();
            _watcher.Path = _config.ProjectWorkingDirectory.Path;
            _watcher.IncludeSubdirectories = true;
            _watcher.EnableRaisingEvents = true;
            _watcher.NotifyFilter = NotifyFilters.DirectoryName | NotifyFilters.FileName | NotifyFilters.CreationTime | NotifyFilters.LastWrite | NotifyFilters.Size;
            _watcher.Changed += (sender, e) => { if (e.ChangeType == WatcherChangeTypes.Changed) WorkingDirectoryChanges(); };
            _watcher.Created += (sender, e) => { if (e.ChangeType == WatcherChangeTypes.Created) WorkingDirectoryChanges(); };
            _watcher.Deleted += (sender, e) => { if (e.ChangeType == WatcherChangeTypes.Deleted) WorkingDirectoryChanges(); };
            _watcher.Renamed += (sender, e) => { if (e.ChangeType == WatcherChangeTypes.Renamed) WorkingDirectoryChanges(); };
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

            _watcher.Dispose();
            _watcher = null;
            _refreshAbort.Release();
        }
        #endregion

        private DateTime _lastWorkingDirectoryChange = new DateTime(2199, 9, 12);
        private void WorkingDirectoryChanges()
        {
            _lastWorkingDirectoryChange = DateTime.Now;
        }

        private void RefreshThreadMain()
        {
            DateTime last = new DateTime(1975, 9, 12);
            while (true)
            {
                DateTime now = DateTime.Now;

                if ((!_refreshSuspended) && 
                    (_refreshNow || 
                     (now - last).TotalSeconds >= 60 ||
                     (now - _lastWorkingDirectoryChange).TotalSeconds >= 3))
                {
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

                    now = DateTime.Now;
                    _refreshNow = false;
                    last = now;
                    _lastWorkingDirectoryChange = new DateTime(2199, 9, 12);
                }

                if (_refreshAbort.WaitOne(500)) break;
            }
        }
    }
}
