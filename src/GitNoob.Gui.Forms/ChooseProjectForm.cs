using GitNoob.Gui.Forms.Properties;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace GitNoob.Gui.Forms
{
    public partial class ChooseProjectForm : Form
    {
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


        private List<Config.IConfig> _configs;
        private string _licenseText;
        private ConcurrentDictionary<Config.WorkingDirectory, WorkingDirectoryForm> _forms;

        private class ProjectWorkingDirectory
        {
            public Config.Project Project { get; set; }
            public Config.WorkingDirectory WorkingDirectory { get; set; }

            public ProjectWorkingDirectory(Config.Project project, Config.WorkingDirectory workingdirectory)
            {
                Project = project;
                WorkingDirectory = workingdirectory;
            }
        }

        public ChooseProjectForm(List<Config.IConfig> configs, string licenseText)
        {
            _configs = configs;
            _licenseText = licenseText;
            _forms = new ConcurrentDictionary<Config.WorkingDirectory, WorkingDirectoryForm>();

            InitializeComponent();
            this.Resize += ChooseProjectForm_Resize;
            this.FormClosing += ChooseProjectForm_FormClosing;
            this.FormClosed += ChooseProjectForm_FormClosed;
            this.Load += ChooseProjectForm_Load;

            BuildForm();
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            // Get a handle to a copy of this form's system (window) menu
            IntPtr hSysMenu = NativeMethods.GetSystemMenu(this.Handle, false);

            // Add a separator
            NativeMethods.AppendMenu(hSysMenu, NativeMethods.MF_SEPARATOR, 0, string.Empty);

            // Add the About menu item
            NativeMethods.AppendMenu(hSysMenu, NativeMethods.MF_STRING, SYSMENU_ABOUT_ID, "&About GitNoob");
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            // Test if the About item was selected from the system menu
            if ((m.Msg == NativeMethods.WM_SYSCOMMAND) && ((int)m.WParam == SYSMENU_ABOUT_ID))
            {
                AboutForm about = new AboutForm(_licenseText);
                about.ShowDialog();
            }
        }

        private void ChooseProjectForm_Load(object sender, EventArgs e)
        {
            //Restore window position
            WindowSizeLocationSaver.Restore(this, Settings.Default.ChooseProjectForm_Position);
            //End restore window position
        }

        private void ChooseProjectForm_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                if (_forms.Count > 0) this.Visible = false;
            }
        }

        private void ChooseProjectForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //Save window position
            Settings.Default.ChooseProjectForm_Position = WindowSizeLocationSaver.Save(this);
            Settings.Default.Save();
            //End save window position

            /*
            foreach (var keyvalue in _forms)
            {
                var form = keyvalue.Value;
                if (form.CanClose())
                {
                    try
                    {
                        form.Close();
                    }
                    catch { }
                }
            }
            */

            if (_forms.Count > 0)
            {
                e.Cancel = true;

                WorkingDirectoryForm form = null;
                foreach (var keyvalue in _forms)
                {
                    form = keyvalue.Value;

                    if (form.WindowState != FormWindowState.Minimized) break;
                }

                if (form.WindowState == FormWindowState.Minimized)
                    form.WindowState = FormWindowState.Normal;
                form.BringToFront();
                form.Activate();
                this.Visible = false;
            }
        }

        private void ChooseProjectForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            int retry = _forms.Count;
            while (_forms.Count > 0 && retry > 0)
            {
                foreach (var keyvalue in _forms)
                {
                    var form = keyvalue.Value;
                    try
                    {
                        form.Close();
                    }
                    catch { }
                }

                retry--;
            }

            Application.Exit();
        }

        private void BuildForm()
        {
            const int margin = 8;
            const int projectheight = 48;
            const int workingdirectoryheight = 96;
            Color background = Color.White;

            this.BackColor = background;

            int right = 0;
            int maxwd = 0;

            int top = margin;
            foreach (var config in _configs)
            {
                foreach (var project in config.GetProjects())
                {
                    int prjLeft = margin;

                    var prjText = new Label
                    {
                        Location = new Point(prjLeft, top),
                        Text = project.Name,
                        AutoSize = true,
                    };
                    prjText.Font = new Font("Arial", 14, FontStyle.Bold);
                    this.Controls.Add(prjText);
                    right = Math.Max(right, prjText.Location.X + prjText.Width);

                    top += projectheight + margin;

                    maxwd = Math.Max(maxwd, project.WorkingDirectories.Count);
                    foreach (var item in project.WorkingDirectories)
                    {
                        int wdLeft = prjLeft;
                        var workingdirectory = item.Value;
                        var prjwd = new ProjectWorkingDirectory(project, workingdirectory);

                        Color color = background;
                        try
                        {
                            color = System.Drawing.ColorTranslator.FromHtml(workingdirectory.ImageBackgroundColor);
                        }
                        catch { }

                        var wdPicture = new PictureBox
                        {
                            Location = new Point(wdLeft, top),
                            Size = new Size(4 * workingdirectoryheight, workingdirectoryheight),
                            Image = Program.Utils.ImageUtils.LoadImageAsBitmap(workingdirectory.ImageFilename.ToString(), 4 * workingdirectoryheight, workingdirectoryheight, color),
                        };
                        wdPicture.Cursor = Cursors.Hand;
                        wdPicture.Tag = prjwd;
                        wdPicture.Click += wdPicture_Click;
                        this.Controls.Add(wdPicture);
                        wdLeft += wdPicture.Size.Width + margin;

                        var wdButton = new Button
                        {
                            Text = workingdirectory.Name,
                            AutoSize = true,
                        };
                        wdButton.Font = new Font("Arial", 12, FontStyle.Bold);
                        wdButton.Location = new Point(wdLeft, top + Convert.ToInt32((float)(workingdirectoryheight - wdButton.Height) / 2f));
                        wdButton.Tag = prjwd;
                        wdButton.Click += wdButton_Click;
                        this.Controls.Add(wdButton);
                        right = Math.Max(right, wdButton.Location.X + wdButton.Width);

                        top += workingdirectoryheight + margin;
                    }
                }
            }

            //Try to show project header + all workingdirectories
            int maxheight = margin + projectheight + margin + (maxwd * (workingdirectoryheight + margin));
            //But don't exceed the screen WorkingArea height
            Rectangle screenRectangle = this.RectangleToScreen(this.ClientRectangle);
            int formExteriorHeight = this.Height - screenRectangle.Height;

            var myScreen = Screen.FromControl(this);
            maxheight = Math.Min(myScreen.WorkingArea.Height - formExteriorHeight, maxheight);

            if (top > maxheight)
            {
                this.ClientSize = new Size(right + margin + SystemInformation.VerticalScrollBarWidth, maxheight);
            }
            else
            {
                this.ClientSize = new Size(right + margin, top);
            }
        }

        private void wdPicture_Click(object sender, EventArgs e)
        {
            startWorkingDirectory((ProjectWorkingDirectory)((PictureBox)sender).Tag);
        }

        private void wdButton_Click(object sender, EventArgs e)
        {
            startWorkingDirectory((ProjectWorkingDirectory)((Button)sender).Tag);
        }

        private void startWorkingDirectory(ProjectWorkingDirectory projectwd)
        {
            WorkingDirectoryForm form;

            if (!_forms.ContainsKey(projectwd.WorkingDirectory))
            {
                try
                {
                    form = new WorkingDirectoryForm(new Program.ProgramWorkingDirectory(projectwd.Project, projectwd.WorkingDirectory), WorkingDirectoryForm_ChooseProject);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Git Noob - open working directory");
                    return;
                }
                _forms.TryAdd(projectwd.WorkingDirectory, form);
                form.FormClosed += WorkingDirectoryForm_FormClosed;
            }
            else
            {
                form = _forms[projectwd.WorkingDirectory];
            }

            form.Visible = true;
            if (form.WindowState == FormWindowState.Minimized)
                form.WindowState = FormWindowState.Normal;
            form.BringToFront();
            form.Activate();
            this.Visible = false;
        }

        private void WorkingDirectoryForm_ChooseProject()
        {
            this.Visible = true;
            this.Focus();
        }

        private void WorkingDirectoryForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            WorkingDirectoryForm form = (WorkingDirectoryForm)sender;

            if (_forms.ContainsKey(form.Config.ProjectWorkingDirectory))
            {
                WorkingDirectoryForm value;
                _forms.TryRemove(form.Config.ProjectWorkingDirectory, out value);
            }

            form = null;
            foreach (var keyvalue in _forms)
            {
                form = keyvalue.Value;
                if (form.WindowState != FormWindowState.Minimized) break;
            }

            if (form == null)
            {
                this.Visible = true;
            }
            else
            {
                if (form.WindowState == FormWindowState.Minimized)
                    form.WindowState = FormWindowState.Normal;
                form.BringToFront();
                form.Activate();
            }
        }
    }
}