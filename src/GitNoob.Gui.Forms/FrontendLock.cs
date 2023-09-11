using System;

namespace GitNoob.Gui.Forms
{
    public class FrontendLock
    {
        private FrontendLockManager _manager;
        private UInt64 _uniqueLockId;
        private bool _locked;
        public FrontendLock(FrontendLockManager Manager, UInt64 UniqueLockId)
        {
            _manager = Manager;
            _uniqueLockId = UniqueLockId;
            _locked = false;
        }

        public void Lock()
        {
            if (_locked) throw new Exception("frontend is already locked");

            _manager.Lock(_uniqueLockId);
            _locked = true;
        }

        public bool IsLocked()
        {
            return _locked;
        }

        public void Unlock()
        {
            if (!_locked) throw new Exception("frontend is not locked");

            _manager.Unlock(_uniqueLockId);
            _locked = false;
        }
    }
}
