using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Automation;
using System.Windows.Forms;

namespace GitNoob.Gui.Forms
{
    public class GitNoobBaseForm : Form
    {
        // Add "About GitNoob" and "Check for GitNoob update" to the form system menu (top left corner).

        protected readonly string _programPath;
        protected readonly string _licenseText;
        private readonly string _GitNoobUpdaterExe;

        private class NativeMethods
        {
            // P/Invoke constants
            public const int WM_SYSCOMMAND = 0x112;
            public const int MF_STRING = 0x0;
            public const int MF_SEPARATOR = 0x800;

            // P/Invoke declarations
            [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            public static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

            [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            public static extern bool AppendMenu(IntPtr hMenu, int uFlags, int uIDNewItem, string lpNewItem);
        }
        // ID for the About item on the system menu
        private int SYSMENU_ABOUT_ID = 0x1;
        private int SYSMENU_CHECKFORUPDATE_ID = 0x2;

        public GitNoobBaseForm(string programPath, string licenseText)
        {
            _programPath = programPath;
            _licenseText = licenseText;
            _GitNoobUpdaterExe = Path.Combine(_programPath, "GitNoobUpdater.exe");
            if (!File.Exists(_GitNoobUpdaterExe)) _GitNoobUpdaterExe = null;

        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            // Get a handle to a copy of this form's system (window) menu
            IntPtr hSysMenu = NativeMethods.GetSystemMenu(this.Handle, false);

            // Add a separator
            NativeMethods.AppendMenu(hSysMenu, NativeMethods.MF_SEPARATOR, 0, string.Empty);

            if (_GitNoobUpdaterExe != null)
            {
                // Add the Check for update item
                NativeMethods.AppendMenu(hSysMenu, NativeMethods.MF_STRING, SYSMENU_CHECKFORUPDATE_ID, "Check for GitNoob update");
            }

            // Add the About menu item
            NativeMethods.AppendMenu(hSysMenu, NativeMethods.MF_STRING, SYSMENU_ABOUT_ID, "About GitNoob");

        }

        private Thread _updaterThread = null;
        private Process _updaterProcess = null;
        private void StartGitNoobUpdater()
        {
            try
            {
                if (_updaterProcess == null || _updaterProcess.HasExited)
                {
                    _updaterProcess = null;
                    if (Debugger.IsAttached)
                    {
                        _updaterProcess = Process.Start(_GitNoobUpdaterExe, "debugger");
                    }
                    else
                    {
                        _updaterProcess = Process.Start(_GitNoobUpdaterExe);
                    }
                }

                {
                    var StartTime = DateTime.Now;
                    var timeoutSeconds = 10;
                    while (_updaterProcess.MainWindowHandle == null || _updaterProcess.MainWindowHandle == IntPtr.Zero)
                    {
                        if (DateTime.Now.Subtract(StartTime).Seconds > timeoutSeconds)
                            throw new Exception("Timeout waiting for GitNoobUpdater.exe MainWindowHandle");

                        Thread.Sleep(100);

                        if (_updaterProcess.HasExited)
                            throw new Exception("GitNoobUpdater.exe has exited");
                    }
                }

                {
                    AutomationElement element = AutomationElement.FromHandle(_updaterProcess.MainWindowHandle);
                    if (element != null)
                    {
                        element.SetFocus();
                    }
                }
            }
            catch { }

            _updaterThread = null;
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            // Test if the Check for update was selected from the system menu
            if ((m.Msg == NativeMethods.WM_SYSCOMMAND) && ((int)m.WParam == SYSMENU_CHECKFORUPDATE_ID))
            {
                if (_updaterThread == null)
                {
                    _updaterThread = new Thread(StartGitNoobUpdater)
                    {
                        Name = "GitNoobUpdater.exe - start"
                    };
                    _updaterThread.Start();

                }
                return;
            }

            // Test if the About item was selected from the system menu
            if ((m.Msg == NativeMethods.WM_SYSCOMMAND) && ((int)m.WParam == SYSMENU_ABOUT_ID))
            {
                AboutForm about = new AboutForm(_licenseText);
                about.ShowDialog();
                return;
            }
        }
    }
}
