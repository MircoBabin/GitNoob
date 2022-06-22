using Microsoft.Win32.SafeHandles;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace GitNoob.ZoneIdentifier
{
    public class IsFileBlocked
    {
        private static class NativeMethods
        {
            [DllImport("kernel32", EntryPoint = "CreateFileW", CharSet = CharSet.Unicode, SetLastError = true)]
            public static extern SafeFileHandle CreateFile(
                string name, FileAccess access, FileShare share,
                IntPtr security,
                FileMode mode, FileAttributes flags,
                IntPtr template);
        }

        public enum FileBlockedStatus { No, Yes, Unknown };
        public class FileBlockedResult
        {
            public FileBlockedStatus Status { get; set; }
            public Exception Exception { get; set; }

            public FileBlockedResult(FileBlockedStatus Status, Exception Exception = null)
            {
                this.Status = Status;
                this.Exception = Exception;
            }
        }
        public static FileBlockedResult CheckFile(string filename)
        {
            try
            {
                // Opens the ":Zone.Identifier" alternate data stream that blocks the file
                using (SafeFileHandle handle = NativeMethods.CreateFile(filename + ":Zone.Identifier", FileAccess.Read, FileShare.ReadWrite, IntPtr.Zero, FileMode.Open, FileAttributes.Normal, IntPtr.Zero))
                {
                    if (handle.IsInvalid) return new FileBlockedResult(FileBlockedStatus.No);
                }

                var persistentZoneIdentifier = new PersistentZoneIdentifier();
                var persistFile = (IPersistFile)persistentZoneIdentifier;
                try
                {
                    persistFile.Load(filename, (int)(STGM.READ | STGM.SHARE_DENY_NONE));
                }
                catch (FileNotFoundException)
                {
                    // When calling persistFile.Load, the object tries to open filename:Zone.Identifier
                    // So, if the file doesn't have an identifier, we get a file not found.
                    return new FileBlockedResult(FileBlockedStatus.No);
                }

                var zoneIdentifier = (IZoneIdentifier)persistentZoneIdentifier;
                var zone = zoneIdentifier.GetId();
                if (zone == UrlZone.LocalMachine)
                {
                    return new FileBlockedResult(FileBlockedStatus.No);
                }

                return new FileBlockedResult(FileBlockedStatus.Yes);
            }
            catch (Exception ex)
            {
                return new FileBlockedResult(FileBlockedStatus.Unknown, ex);
            }
        }
    }
}
