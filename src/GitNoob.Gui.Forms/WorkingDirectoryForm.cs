using GitNoob.Gui.Forms.Properties;
using GitNoob.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace GitNoob.Gui.Forms
{
    public partial class WorkingDirectoryForm : GitNoobBaseForm, Visualizer.IVisualizer
    {
        public Visualizer.IVisualizerProgram Config { get; }
        public Config.WorkingDirectory WorkingDirectory { get; }

        private WorkingDirectoryRefreshStatus _refresh;
        private System.Action _chooseProject;

        private int _originalHeight;

        private FrontendLock _frontendLock;


        public WorkingDirectoryForm(Visualizer.IVisualizerProgram config,
            FrontendLock frontendLock,
            Config.WorkingDirectory workingDirectory,
            System.Action chooseProject, string programPath, string licenseText, string rootConfigurationFilename) :
            base(programPath, licenseText, rootConfigurationFilename)
        {
            _frontendLock = frontendLock;
            WorkingDirectory = workingDirectory;
            Config = config;

            Config.visualizerSet(this);
            _chooseProject = chooseProject;

            InitializeComponent();

            BuildForm();
            this.FormClosed += WorkingDirectoryForm_FormClosed;
            this.Load += WorkingDirectoryForm_Load;

            _originalHeight = this.Height;

            _refresh = null;
            this.VisibleChanged += WorkingDirectoryForm_VisibleChanged;
        }

        private void WorkingDirectoryForm_Load(object sender, EventArgs e)
        {
            RestoreWindowPosition();
        }

        private void WorkingDirectoryForm_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible && _refresh == null)
            {
                lockFrontend();
                busyMessage("Busy - retrieving status");

                _refresh = new WorkingDirectoryRefreshStatus(Config, OnStatusRefreshed, OnStatusRefreshedError);
            }
        }

        public bool CanClose()
        {
            return !isFrontendLocked();
        }

        private void WorkingDirectoryForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveWindowPosition();

            if (!CanClose())
            {
                e.Cancel = true;
            }
        }

        private void WorkingDirectoryForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (_refresh != null)
            {
                _refresh.Dispose();
                _refresh = null;
            }

            Config.visualizerExit();
        }

        private void OnChangeBranch()
        {
            Config.visualizerChangeBranch().execute();
        }

        private void lblCurrentbranchValue_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LinkLabelUtils.ExecuteLinkClicked(e);
        }

        public void notifyCurrentBranchChanged(string branchname)
        {
            ShowCurrentBranch(branchname, true);
        }

        private void ShowCurrentBranch(string branchname, bool refreshNow)
        {
            if (!this.Visible) return;
            this.Invoke((MethodInvoker)delegate
            {
                if (refreshNow)
                {
                    _refresh.RefreshNow();
                }

                if (String.IsNullOrWhiteSpace(branchname))
                {
                    branchname = "...";
                }

                LinkLabelUtils.SetLinkLabel(lblCurrentbranchValue, "[link changebranch]" + branchname + "[/link]",
                    new Dictionary<string, System.Action>()
                    {
                        { "changebranch", OnChangeBranch }
                    });
            });
        }

        private void OnStatusRefreshed(GitResult.StatusResult status)
        {
            if (!this.Visible) return;
            this.Invoke((MethodInvoker)delegate
            {
                if (status.DetachedHead_NotOnBranch)
                    ShowCurrentBranch("[detached HEAD]", false);
                else
                    ShowCurrentBranch(status.CurrentBranch, false);

                lblCommitnameValue.Text = status.CommitFullName;
                if (status.ClearCommitNameAndEmailOnExit)
                    lblCommitnameValue.Text += " [clear on exit]";

                StringBuilder txt = new StringBuilder();
                if (status.DirectoryExists)
                {
                    if (status.IsGitRootDirectory)
                    {
                        if (status.Rebasing) txt.Append("rebasing, ");
                        if (status.Merging) txt.Append("merging, ");
                        if (status.CherryPicking) txt.Append("cherry-picking, ");
                        if (status.Reverting) txt.Append("reverting, ");
                        if (status.Conflicts) txt.Append("conflicts, ");

                        if (status.DetachedHead_NotOnBranch) txt.Append("not on a branch [detached HEAD], ");

                        if (status.HasWorkingTreeChanges)
                            txt.Append("changes, ");
                        else
                            txt.Append("no changes, ");

                        if (status.HasStagedUncommittedFiles) txt.Append("staged files, ");
                    }
                    else
                    {
                        txt.Append("not a git repository root directory, ");
                    }
                }
                else
                {
                    txt.Append("directory does not exist, ");
                }

                txt.Remove(txt.Length - 2, 2);

                lblStatusValue.Text = txt.ToString();

                AfterRefresh(status);
            });
        }

        private void OnStatusRefreshedError(Exception ex)
        {
            if (!this.Visible) return;
            Invoke((MethodInvoker)delegate
            {
                lblStatusValue.Text = "error: " + ex.Message;

                AfterRefresh(null);
            });
        }

        private Object _firstRefresh_LockObj = new Object();
        private volatile bool _firstRefresh = true;
        private void AfterRefresh(GitResult.StatusResult status)
        {
            lock (_firstRefresh_LockObj)
            {
                if (_firstRefresh)
                {
                    _firstRefresh = false;
                    if (isFrontendLocked())
                        unlockFrontend();

                    if (status != null)
                    {
                        Config.visualizerReady(status);
                    }
                }
            }
        }

        private string _positionSettingsName;
        private void SaveWindowPosition()
        {
            if (Settings.Default.WorkingDirectoryForm_Positions == null)
            {
                //Create an empty list in settings editor (Project Properties, settings, edit value)
                Settings.Default.WorkingDirectoryForm_Positions = new System.Collections.Specialized.StringCollection();
            }

            string store = _positionSettingsName + "=" + WindowSizeLocationSaver.Save(this);

            for (int i = 0; i < Settings.Default.WorkingDirectoryForm_Positions.Count; i++)
            {
                string str = Settings.Default.WorkingDirectoryForm_Positions[i];
                if (!string.IsNullOrWhiteSpace(str))
                {
                    var p = str.IndexOf('=');
                    if (p >= 0)
                    { 
                        string name = str.Substring(0, p);
                        string pos = str.Substring(p + 1);

                        if (name == _positionSettingsName)
                        {
                            Settings.Default.WorkingDirectoryForm_Positions[i] = store;
                            Settings.Default.Save();
                            return;
                        }
                    }
                }
            }

            Settings.Default.WorkingDirectoryForm_Positions.Add(store);
            Settings.Default.Save();
        }

        private void RestoreWindowPosition()
        {
            if (Settings.Default.WorkingDirectoryForm_Positions == null) return;

            for (int i = 0; i < Settings.Default.WorkingDirectoryForm_Positions.Count; i++)
            {
                string str = Settings.Default.WorkingDirectoryForm_Positions[i];
                if (!string.IsNullOrWhiteSpace(str))
                {
                    var p = str.IndexOf('=');
                    if (p >= 0)
                    {
                        string name = str.Substring(0, p);
                        string pos = str.Substring(p + 1);

                        if (name == _positionSettingsName)
                        {
                            WindowSizeLocationSaver.Restore(this, pos);
                            return;
                        }
                    }
                }
            }
        }

        private void BuildForm()
        {
            _positionSettingsName = BitConverter.ToString(Encoding.UTF8.GetBytes(Config.visualizerProjectName() + " - " + Config.visualizerProjectWorkingDirectoryName())).Replace("-", String.Empty);

            string filename;

            filename = Config.visualizerProjectWorkingDirectoryIconFilename();
            if (!String.IsNullOrEmpty(filename))
            {
                this.Icon = new Icon(filename);
            }


            this.Text = Config.visualizerProjectName() + " - " + Config.visualizerProjectWorkingDirectoryName();

            Color color = Color.White;
            try
            {
                color = System.Drawing.ColorTranslator.FromHtml(Config.visualizerProjectWorkingDirectoryImageBackgroundHtmlColor());
            }
            catch { }
            Picture.Image = ImageUtils.LoadImageAsBitmap(Config.visualizerProjectWorkingDirectoryImageFilename(), Picture.ClientSize.Width, Picture.ClientSize.Height, color);
            if (_chooseProject != null)
            {
                Picture.Cursor = Cursors.Hand;
                Picture.MouseClick += Picture_MouseClick;
                toolTips.SetToolTip(Picture, "Click to open another project.");
            }

            lblMainbranchValue.Text = Config.visualizerProjectWorkingDirectoryMainBranch();
            if (Config.visualizerProjectWorkingDirectoryTouchTimestampsBeforeMerge())
            {
                lblMainbranchValue.Text = lblMainbranchValue.Text + " [touch timestamps before merge]";
            }
            lblWorkingdirectoryValue.Text = Config.visualizerProjectWorkingDirectoryPath();
            lblCurrentbranchValue.Text = String.Empty;
            lblCommitnameValue.Text = String.Empty;

            lblStatusValue.Text = String.Empty;

            panelStatus.Left = 0;
            panelStatus.Width = this.ClientSize.Width;
            panelStatus.Height = this.ClientSize.Height - panelStatus.Top;
            panelStatus.Visible = true;

            panelError.Visible = false;
            panelError.Location = panelStatus.Location;
            panelError.Size = panelStatus.Size;
            panelError.AutoScroll = true;
            errorPicture.ClientSize = new Size(48, 48);
            errorPicture.Image = ImageUtils.IconToBitmapOfSize(Resources.getIcon("error"), 48, 48, Color.Transparent);

            labelBusy.Visible = false;
            labelBusy.MaximumSize = new Size(panelStatus.Width, 0);
            labelBusy.AutoSize = true;

            Point location = new Point(lblStatusValue.Location.X, lblStatusValue.Location.Y + 26);

            Point empty = new Point(0,0);
            { 
                new ActionButton(null, "", null, null, ref empty);
            }

            Visualizer.IViusalizerAction action;
            action = Config.visualizerStartBrowser();
            if (action.isStartable())
            {
                this.panelStatus.Controls.Add(new ActionButton(toolTips, "Browser", null, action, ref location));
            }

            action = Config.visualizerStartExplorer();
            if (action.isStartable())
            {
                this.panelStatus.Controls.Add(new ActionButton(toolTips, "Explorer", null, action, ref location));
            }

            action = Config.visualizerStartWorkspace();
            if (action.isStartable())
            {
                this.panelStatus.Controls.Add(new ActionButton(toolTips, "Editor", null, action, ref location));
            }

            action = Config.visualizerStartDosPromptAsUser();
            if (action.isStartable())
            {
                var DosPromptContext = new ContextMenu();
                {
                    var item = new MenuItem()
                    {
                        Text = "Run as administrator",
                    };

                    item.Click += (object sender, EventArgs e) =>
                    {
                        Config.visualizerStartDosPromptAsAdministrator().execute();
                    };

                    DosPromptContext.MenuItems.Add(item);
                }
                this.panelStatus.Controls.Add(new ActionButton(toolTips, "DOS prompt", DosPromptContext, action, ref location));
            }

            action = Config.visualizerStartGitGui();
            if (action.isStartable())
            {
                this.panelStatus.Controls.Add(new ActionButton(toolTips, "Git Gui", null, action, ref location));
            }

            //
            // Position git actions under MainBranch
            //
            if (lblMainbranch.Location.X > location.X) location.X = lblMainbranch.Location.X;

            action = Config.visualizerStartGitkForCurrentBranch();
            if (action.isStartable())
            {
                var HistoryContext = new ContextMenu();
                {
                    var item = new MenuItem()
                    {
                        Text = "Show history of one file",
                    };

                    item.Click += (object sender, EventArgs e) =>
                    {
                        using (OpenFileDialog openFileDialog = new OpenFileDialog())
                        {
                            openFileDialog.Title = "Show history of one file";
                            openFileDialog.InitialDirectory = Config.visualizerProjectWorkingDirectoryPath();
                            openFileDialog.Filter = "All files (*.*)|*.*";

                            if (openFileDialog.ShowDialog() == DialogResult.OK)
                            {
                                Config.visualizerStartGitkForOneFile(openFileDialog.FileName);
                            }
                        }
                    };

                    HistoryContext.MenuItems.Add(item);

                    item = new MenuItem()
                    {
                        Text = "Show history of all branches / tags / remotes",
                    };

                    item.Click += (object sender, EventArgs e) =>
                    {
                        Config.visualizerStartGitkAll().execute();
                    };

                    HistoryContext.MenuItems.Add(item);

                }
                this.panelStatus.Controls.Add(new ActionButton(toolTips, "Current branch history", HistoryContext, action, ref location));
            }

            location.X += empty.X;

            action = Config.visualizerGetLatest();
            if (action.isStartable())
            {
                this.panelStatus.Controls.Add(new ActionButton(toolTips, "Get latest", null, action, ref location));
            }

            action = Config.visualizerCherryPick();
            if (action.isStartable())
            {
                this.panelStatus.Controls.Add(new ActionButton(toolTips, "Cherry pick one commit", null, action, ref location));
            }

            action = Config.visualizerMerge();
            if (action.isStartable())
            {
                this.panelStatus.Controls.Add(new ActionButton(toolTips, "Merge", null, action, ref location));
            }

            location.X += empty.X;

            action = Config.visualizerDeleteAllChanges();
            if (action.isStartable())
            {
                this.panelStatus.Controls.Add(new ActionButton(toolTips, "Delete all changes", null, action, ref location));
            }

            action = Config.visualizerGitRepairOptions();
            if (action.isStartable())
            {
                this.panelStatus.Controls.Add(new ActionButton(toolTips, "Repair options", null, action, ref location));
            }

            //
            // Second row
            //
            location.X = lblStatusValue.Location.X;
            location.Y += 70;
            int count = 0;

            action = Config.visualizerStartExploreLogFiles();
            if (action.isStartable())
            {
                this.panelStatus.Controls.Add(new ActionButton(toolTips, "Explore logfiles", null, action, ref location));
                count++;
            }

            action = Config.visualizerOpenConfigFiles();
            if (action.isStartable())
            {
                this.panelStatus.Controls.Add(new ActionButton(toolTips, "Open configfiles", null, action, ref location));
                count++;
            }

            if (count>0) location.X += empty.X;
            count = 0;

            action = Config.visualizerClearCache();
            if (action.isStartable())
            {
                this.panelStatus.Controls.Add(new ActionButton(toolTips, "Clear cache", null, action, ref location));
                count++;
            }

            action = Config.visualizerDeleteLogFiles();
            if (action.isStartable())
            {
                this.panelStatus.Controls.Add(new ActionButton(toolTips, "Delete logfiles", null, action, ref location));
                count++;
            }

            //
            // Third row
            //
            location.X = lblStatusValue.Location.X;
            location.Y += 70;

            action = Config.visualizerStartSmtpServer();
            if (action.isStartable())
            {
                this.panelStatus.Controls.Add(new ActionButton(toolTips, "Smtp Server", null, action, ref location));
            }

            action = Config.visualizerStartFiddler();
            if (action.isStartable())
            {
                this.panelStatus.Controls.Add(new ActionButton(toolTips, "Fiddler", null, action, ref location));
            }

            action = Config.visualizerStartNgrok();
            if (action.isStartable())
            {
                this.panelStatus.Controls.Add(new ActionButton(toolTips, "Ngrok", null, action, ref location));
            }
        }

        private void Picture_MouseClick(object sender, MouseEventArgs e)
        {
            if (_chooseProject != null) _chooseProject();
        }

        public void copyToClipboard(string text)
        {
            Clipboard.SetText(text);
        }

        public void lockFrontend()
        {
            _frontendLock.Lock();

            if (_refresh != null)  _refresh.Suspend();
            showLocked();
        }

        public bool isFrontendLocked()
        {
            return _frontendLock.IsLocked();
        }

        private void showLocked()
        {
            if (!this.Visible) return;
            this.Invoke((MethodInvoker)delegate
            {
                labelBusy.Text = String.Empty;
                labelBusy.Top = panelStatus.Top + ((panelStatus.Height - labelBusy.Height) / 2);
                labelBusy.Left = panelStatus.Left + ((panelStatus.Width - labelBusy.Width) / 2);
                labelBusy.Visible = true;

                lblCurrentbranchValue.Enabled = false;
                panelError.Visible = false;
                panelStatus.Visible = false;

                this.Height = _originalHeight;
            });
        }

        private void backToStatus()
        {
            if (_frontendLock.IsLocked())
            {
                showLocked();
                return;
            }

            if (!this.Visible) return;
            this.Invoke((MethodInvoker)delegate
            {
                lblCurrentbranchValue.Enabled = true;
                labelBusy.Visible = false;
                panelError.Visible = false;
                panelStatus.Visible = true;

                this.Height = _originalHeight;
            });
        }

        public void unlockFrontend()
        {
            _frontendLock.Unlock();

            _refresh.Resume();
            backToStatus();
        }

        public void showException(Exception ex)
        {
            this.Invoke((MethodInvoker)delegate
            {
                MessageBox.Show(ex.Message, "GitNoob - Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            });
        }

        public void busyMessage(string message)
        {
            if (!this.Visible) return;

            this.Invoke((MethodInvoker)delegate
            {
                if (String.IsNullOrWhiteSpace(message))
                {
                    labelBusy.Text = String.Empty;
                    backToStatus();
                }
                else
                {
                    labelBusy.Text = message;
                    labelBusy.Top = panelStatus.Top + ((panelStatus.Height - labelBusy.Height) / 2);
                    labelBusy.Left = panelStatus.Left + ((panelStatus.Width - labelBusy.Width) / 2);
                    labelBusy.Visible = true;

                    lblCurrentbranchValue.Enabled = false;
                    panelError.Visible = false;
                    panelStatus.Visible = false;

                    this.Height = _originalHeight;
                }
            });
        }

        #region ErrorPanel
        private List<ErrorButton> _errorButtons = new List<ErrorButton>();

        private void errorText_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LinkLabelUtils.ExecuteLinkClicked(e);
        }

        private void showErrorPanel(Visualizer.IVisualizerMessage message)
        {
            if (null == message.VisualizerMessageButtons || message.VisualizerMessageButtons.Count == 0)
            {
                throw new Exception("buttons should have at least 1 entry");
            }

            panelError.AutoScroll = false;
            while (_errorButtons.Count < message.VisualizerMessageButtons.Count)
            {
                var button = new ErrorButton(this.panelError.Controls);

                _errorButtons.Add(button);
            };

            lblCurrentbranchValue.Enabled = false;
            labelBusy.Visible = false;
            panelStatus.Visible = false;

            var size = new Size(panelStatus.ClientSize.Width - SystemInformation.VerticalScrollBarWidth -
                                errorText.Left -
                                (errorText.Left - (errorPicture.Left + errorPicture.Width)), 0);
            errorText.MaximumSize = size;
            errorText.AutoSize = true;

            LinkLabelUtils.SetLinkLabel(errorText, message.VisualizerMessageText.Message.ToString(), message.VisualizerMessageText.Links);

            errorText2.MaximumSize = size;
            errorText2.AutoSize = true;

            LinkLabelUtils.SetLinkLabel(errorText2, message.VisualizerMessageInput2.Message.ToString(), message.VisualizerMessageInput2.Links);

            Point location;
            TextBox input;
            TextBox input2 = null;
            errorText2.Visible = false;
            errorInput2.Visible = false;
            switch (message.VisualizerMessageType)
            {
                case Visualizer.IVisualizerMessageType.options:
                    errorInput.Visible = false;

                    location = new Point(errorText.Left, errorText.Top + errorText.Height + 30);
                    input = null;
                    break;

                case Visualizer.IVisualizerMessageType.input:
                case Visualizer.IVisualizerMessageType.input2:
                    location = new Point(errorText.Left, errorText.Top + errorText.Height + 5);
                    errorInput.Location = location;
                    errorInput.Width = errorText.MaximumSize.Width;
                    errorInput.Text = String.Empty;
                    errorInput.Visible = true;

                    location = new Point(errorInput.Left, errorInput.Top + errorInput.Height + 30);
                    input = errorInput;

                    if (message.VisualizerMessageType == Visualizer.IVisualizerMessageType.input2)
                    {
                        errorText2.Location = location;
                        errorText2.Visible = true;
                        location = new Point(errorText2.Left, errorText2.Top + errorText2.Height + 5);

                        errorInput2.Location = location;
                        errorInput2.Width = errorText.MaximumSize.Width;
                        errorInput2.Text = String.Empty;
                        errorInput2.Visible = true;

                        location = new Point(errorInput2.Left, errorInput2.Top + errorInput2.Height + 30);
                        input2 = errorInput2;
                    }
                    break;

                default:
                    throw new Exception("Unknown IVisualizerMessageType");
            }

            {
                int no = 0;
                foreach (var item in message.VisualizerMessageButtons)
                {
                    if (item != null)
                    {
                        ErrorButton button = _errorButtons[no];
                        button.ShowErrorButton(toolTips, input, input2, item, ref location, size);

                        no++;
                    }
                }
                while (no < _errorButtons.Count)
                {
                    ErrorButton button = _errorButtons[no];
                    button.HideErrorButton();

                    no++;
                }
            }

            errorBottom.Text = String.Empty;
            errorBottom.Top = (location.Y + 10) - errorBottom.Height;

            //Try to show full message
            int maxheight = panelError.Top + errorBottom.Bottom;
            this.ClientSize = new Size(this.ClientSize.Width, maxheight);
            //But don't exceed the screen WorkingArea height
            var myScreen = Screen.FromControl(this);
            if (this.Height > myScreen.WorkingArea.Height) this.Height = myScreen.WorkingArea.Height;

            panelError.Height = this.ClientSize.Height - panelError.Top;
            panelError.Visible = true;
            panelError.AutoScroll = true;

            if (message.VisualizerMessageType == Visualizer.IVisualizerMessageType.input)
            {
                errorInput.Focus();
            }
        }

        public void message(Visualizer.IVisualizerMessage message)
        {
            if (!this.Visible) return;

            this.Invoke((MethodInvoker)delegate
            {
                showErrorPanel(message);
            });
        }

        #endregion
    }
}
