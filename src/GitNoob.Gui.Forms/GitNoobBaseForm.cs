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
        // Add to the form system menu (top left corner):
        // - About GitNoob
        // - Check for GitNoob update
        // - Edit GitNoob root configuration file
        // - Edit GitNoob project configuration file

        protected readonly string _programPath;
        protected readonly string _licenseText;
        protected readonly string _rootConfigurationFilename;
        protected readonly string _projectConfigurationFilename;
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
        private int SYSMENU_EDITROOTCONFIGURATION_ID = 0x3;
        private int SYSMENU_EDITPROJECTCONFIGURATION_ID = 0x4;

        public GitNoobBaseForm()
        {
            _programPath = string.Empty;
            _licenseText = string.Empty;
            _GitNoobUpdaterExe = null;
        }

        public GitNoobBaseForm(string programPath, string licenseText, string rootConfigurationFilename, string projectConfigurationFilename)
        {
            _programPath = programPath;
            _licenseText = licenseText;
            _rootConfigurationFilename = rootConfigurationFilename;
            _projectConfigurationFilename = projectConfigurationFilename;
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

            // Add the Edit GitNoob root configuration file menu item
            NativeMethods.AppendMenu(hSysMenu, NativeMethods.MF_STRING, SYSMENU_EDITROOTCONFIGURATION_ID, "Edit GitNoob root configuration file");

            if (!string.IsNullOrWhiteSpace(_projectConfigurationFilename))
            {
                // Add the Edit GitNoob project configuration file menu item
                NativeMethods.AppendMenu(hSysMenu, NativeMethods.MF_STRING, SYSMENU_EDITPROJECTCONFIGURATION_ID, "Edit GitNoob project configuration file");
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

            // Test if the Edit GitNoob root configuration file item was selected from the system menu
            if ((m.Msg == NativeMethods.WM_SYSCOMMAND) && ((int)m.WParam == SYSMENU_EDITROOTCONFIGURATION_ID))
            {
                try
                {
                    Utils.BatFile.StartEditor(null, new string[] { _rootConfigurationFilename });
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "GitNoob - Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return;
            }

            // Test if the Edit GitNoob project configuration file item was selected from the system menu
            if ((m.Msg == NativeMethods.WM_SYSCOMMAND) && ((int)m.WParam == SYSMENU_EDITPROJECTCONFIGURATION_ID))
            {
                try
                {
                    Utils.BatFile.StartEditor(null, new string[] { _projectConfigurationFilename });
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "GitNoob - Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
