using System;
using System.Runtime.InteropServices;

namespace GitNoob.Gui.Forms
{
    public static class WindowsKeepActive
    {
        private static class NativeMethods
        {

            [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            public static extern ExecutionState SetThreadExecutionState(ExecutionState esFlags);

            [FlagsAttribute]
            public enum ExecutionState : uint
            {
                EsAwaymodeRequired = 0x00000040,
                EsContinuous = 0x80000000,
                EsDisplayRequired = 0x00000002,
                EsSystemRequired = 0x00000001
            }
        }

        public static void PreventSleep()
        {
            try
            {
                NativeMethods.SetThreadExecutionState(NativeMethods.ExecutionState.EsContinuous |
                                                      NativeMethods.ExecutionState.EsSystemRequired |
                                                      NativeMethods.ExecutionState.EsDisplayRequired);
            }
            catch { }
        }

        public static void AllowSleep()
        {
            try
            {
                NativeMethods.SetThreadExecutionState(NativeMethods.ExecutionState.EsContinuous);
            }
            catch { }
        }
    }
}
