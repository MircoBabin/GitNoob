﻿using GitNoob.Gui.Forms.Properties;
using GitNoob.Utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace GitNoob.Gui.Forms
{
    public partial class ChooseProjectForm : GitNoobBaseForm
    {
        private Visualizer.IVisualizerBootstrapper _bootstrapper;
        private List<Config.IConfig> _configs;
        private ConcurrentDictionary<Config.WorkingDirectory, WorkingDirectoryForm> _forms;
        private FrontendLockManager _lockManager;

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

        public ChooseProjectForm(Visualizer.IVisualizerBootstrapper Bootstrapper, List<Config.IConfig> configs, string programPath, string licenseText, string rootConfigurationFilename) : 
            base(programPath, licenseText, rootConfigurationFilename, null)
        {
            _bootstrapper = Bootstrapper;
            _configs = configs;
            _forms = new ConcurrentDictionary<Config.WorkingDirectory, WorkingDirectoryForm>();
            _lockManager = new FrontendLockManager();

            InitializeComponent();
            this.Resize += ChooseProjectForm_Resize;
            this.FormClosing += ChooseProjectForm_FormClosing;
            this.FormClosed += ChooseProjectForm_FormClosed;
            this.Load += ChooseProjectForm_Load;

            BuildForm();
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

            _lockManager.Dispose();

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

            bool first = true;

            int top = margin;
            foreach (var config in _configs)
            {
                foreach (var project in config.GetProjects())
                {
                    int prjLeft = margin;

                    Label prjTopLine = null;
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        prjTopLine = new Label
                        {
                            Location = new Point(prjLeft, top),
                            Text = string.Empty,
                            AutoSize = false,
                            BorderStyle = BorderStyle.Fixed3D,
                            Height = 2,
                        };
                        this.Controls.Add(prjTopLine);
                        top += 5;
                    }

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
                            Image = ImageUtils.LoadImageAsBitmap(workingdirectory.ImageFilename.ToString(), 4 * workingdirectoryheight, workingdirectoryheight, color),
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

            foreach(var control in this.Controls)
            {
                if (control is Label)
                {
                    Label label = control as Label;
                    if (label.Height == 2 && label.AutoSize == false)
                    {
                        label.Width = right;
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
                    form = new WorkingDirectoryForm(_bootstrapper.CreateIVisualizerProgram(projectwd.Project, projectwd.WorkingDirectory),
                        _lockManager.NewFrontendLock(),
                        projectwd.WorkingDirectory,
                        WorkingDirectoryForm_ChooseProject, _programPath, _licenseText, _rootConfigurationFilename);
                }
                catch (Exception ex)
                {
                    Clipboard.SetText(ex.ToString());
                    MessageBox.Show(ex.Message + Environment.NewLine + Environment.NewLine + "Error details are copied to the Windows clipboard.", "GitNoob - open working directory error");
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

            if (_forms.ContainsKey(form.WorkingDirectory))
            {
                WorkingDirectoryForm value;
                _forms.TryRemove(form.WorkingDirectory, out value);
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