using System;
using System.Collections.Generic;
using System.Threading;

namespace GitNoob.Gui.Forms
{
    public class FrontendLockManager
    {
        private Object _lockObj = new Object();
        private UInt64 _uniqueLockIdNo = 0;
        private Dictionary<UInt64, bool> _locked = new Dictionary<UInt64, bool>();

        private Thread _lockThread = null;
        private Semaphore _lockThreadAbort = new Semaphore(0, int.MaxValue);


        public FrontendLockManager()
        {
            _lockThread = new Thread(LockThreadMain);
            _lockThread.Name = "FrontendLockManagerThread";
            _lockThread.Start();
        }

        #region IDisposable
        public void Dispose()
        {
            Dispose(true);
        }

        ~FrontendLockManager()
        {
            Dispose(false);
        }

        protected volatile bool _isDisposed = false;
        protected void Dispose(bool disposing)
        {
            if (_isDisposed) return;
            _isDisposed = true;

            _lockThreadAbort.Release();
        }
        #endregion


        private void LockThreadMain()
        {
            DateTime last = new DateTime(1975, 9, 12);
            bool lastLocked = false;

            while (true)
            {
                if (_lockThreadAbort.WaitOne(1000)) break;

                DateTime now = DateTime.Now;
                bool locked;
                lock (_lockObj)
                {
                    locked = (_locked.Count > 0);
                }

                if (locked != lastLocked)
                {
                    if (locked)
                        WindowsKeepActive.PreventSleep();
                    else
                        WindowsKeepActive.AllowSleep();
                }
                else
                {
                    if (locked && (now - last).Seconds >= 10)
                        WindowsKeepActive.PreventSleep();
                }

                lastLocked = locked;
                last = now;
            }
        }


        public FrontendLock NewFrontendLock()
        {
            UInt64 uniqueLockId;

            lock (_lockObj)
            {
                uniqueLockId = _uniqueLockIdNo++;
            }

            return new FrontendLock(this, uniqueLockId);
        }

        public void Lock(UInt64 LockId)
        {
            lock (_lockObj)
            {
                _locked.Add(LockId, true);
            }

        }

        public void Unlock(UInt64 LockId)
        {
            lock (_lockObj)
            {
                _locked.Remove(LockId);
            }
        }
    }
}
