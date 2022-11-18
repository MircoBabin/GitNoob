using GitNoob.Gui.Forms.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace GitNoob.Gui.Forms
{
    public partial class WorkingDirectoryForm : GitNoobBaseForm, Program.Action.IVisualizer
    {
        public Program.ProgramWorkingDirectory Config { get; }

        private WorkingDirectoryRefreshStatus _refresh;
        private System.Action _chooseProject;

        private int _originalHeight;

        public WorkingDirectoryForm(Program.ProgramWorkingDirectory config, System.Action chooseProject, string programPath, string licenseText) :
            base(programPath, licenseText)
        {
            Config = config;
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

            if (Config.ProjectWorkingDirectory.Git.ClearCommitNameAndEmailOnExit.Value)
            {
                Config.Git.ClearCommitter();
            }
        }

        Program.Action.ExecuteChangeBranch ActionChangeBranch = null;
        private void OnChangeBranch()
        {
            if (ActionChangeBranch == null) return;

            ActionChangeBranch.execute();
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

        private void OnStatusRefreshed(Git.Result.StatusResult status)
        {
            if (!this.Visible) return;
            this.Invoke((MethodInvoker)delegate
            {
                if (status.DetachedHead_NotOnBranch)
                    ShowCurrentBranch("[detached HEAD]", false);
                else
                    ShowCurrentBranch(status.CurrentBranch, false);

                lblCommitnameValue.Text = status.CommitFullName;
                if (Config.ProjectWorkingDirectory.Git.ClearCommitNameAndEmailOnExit.Value)
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
        Program.Action.ExecuteAfterStatus ActionAfterStatus = null;
        private void AfterRefresh(Git.Result.StatusResult status)
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
                        ActionAfterStatus.execute(status);
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
            _positionSettingsName = BitConverter.ToString(Encoding.UTF8.GetBytes(Config.Project.Name + " - " + Config.ProjectWorkingDirectory.Name)).Replace("-", String.Empty);

            string filename;

            filename = Config.ProjectWorkingDirectory.IconFilename.ToString();
            if (File.Exists(filename))
            {
                this.Icon = new Icon(filename);
            }
            else
            {
                filename = Config.Project.IconFilename.ToString();
                if (File.Exists(filename))
                {
                    this.Icon = new Icon(filename);
                }
            }

            this.Text = Config.Project.Name + " - " + Config.ProjectWorkingDirectory.Name;

            Color color = Color.White;
            try
            {
                color = System.Drawing.ColorTranslator.FromHtml(Config.ProjectWorkingDirectory.ImageBackgroundColor);
            }
            catch { }
            Picture.Image = Program.Utils.ImageUtils.LoadImageAsBitmap(Config.ProjectWorkingDirectory.ImageFilename.ToString(), Picture.ClientSize.Width, Picture.ClientSize.Height, color);
            if (_chooseProject != null)
            {
                Picture.Cursor = Cursors.Hand;
                Picture.MouseClick += Picture_MouseClick;
            }

            lblMainbranchValue.Text = Config.ProjectWorkingDirectory.Git.MainBranch;
            lblWorkingdirectoryValue.Text = Config.ProjectWorkingDirectory.Path.ToString();
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
            errorPicture.Image = Program.Utils.ImageUtils.IconToBitmapOfSize(Program.Utils.Resources.getIcon("error"), 48, 48, Color.Transparent);

            labelBusy.Visible = false;
            labelBusy.MaximumSize = new Size(panelStatus.Width, 0);
            labelBusy.AutoSize = true;

            Point location = new Point(lblStatusValue.Location.X, lblStatusValue.Location.Y + 26);

            Program.Action.StepsExecutor.StepConfig StepConfig = new Program.Action.StepsExecutor.StepConfig(Config, this, 
                new Program.Utils.BatFile(
                    "iexecutor",
                    Program.Utils.BatFile.RunAsType.runAsInvoker, Program.Utils.BatFile.WindowType.hideWindow,
                    "ProjectType - Executor",
                    Config.Project, Config.ProjectWorkingDirectory,
                    Config.PhpIni));
            ActionChangeBranch = new Program.Action.ExecuteChangeBranch(StepConfig);
            ActionAfterStatus = new Program.Action.ExecuteAfterStatus(StepConfig);

            Point empty = new Point(0,0);
            { 
                new ActionButton(null, "", null, null, ref empty);
            }

            var browser = new Program.Action.ExecuteStartBrowser(StepConfig);
            if (browser.isStartable())
            {
                this.panelStatus.Controls.Add(new ActionButton(toolTips, "Browser", null, browser, ref location));
            }

            this.panelStatus.Controls.Add(new ActionButton(toolTips, "Explorer", null, new Program.Action.StartExplorer(StepConfig), ref location));

            var workspace = new Program.Action.ExecuteWorkspace(StepConfig);
            if (workspace.isStartable())
            {
                this.panelStatus.Controls.Add(new ActionButton(toolTips, "Editor", null, workspace, ref location));
            }

            var DosPrompt = new Program.Action.StartDosPrompt(StepConfig);
            var DosPromptContext = new ContextMenu();
            {
                var item = new MenuItem()
                {
                    Text = "Run as administrator",
                };

                item.Click += (object sender, EventArgs e) =>
                {
                    DosPrompt.executeAsAdministrator();
                };

                DosPromptContext.MenuItems.Add(item);
            }
            this.panelStatus.Controls.Add(new ActionButton(toolTips, "DOS prompt", DosPromptContext, DosPrompt, ref location));
            this.panelStatus.Controls.Add(new ActionButton(toolTips, "Git Gui", null, new Program.Action.StartGitGui(StepConfig), ref location));

            if (lblMainbranch.Location.X > location.X) location.X = lblMainbranch.Location.X;
            var HistoryContext = new ContextMenu();
            {
                var item = new MenuItem()
                {
                    Text = "Show history of one file",
                };

                item.Click += (object sender, EventArgs e) =>
                {
                    ViewHistoryOfFile(StepConfig);
                };

                HistoryContext.MenuItems.Add(item);

                item = new MenuItem()
                {
                    Text = "Show history of all branches / tags / remotes",
                };

                item.Click += (object sender, EventArgs e) =>
                {
                    ViewHistoryOfAll(StepConfig);
                };

                HistoryContext.MenuItems.Add(item);

            }
            this.panelStatus.Controls.Add(new ActionButton(toolTips, "Current branch history", HistoryContext, new Program.Action.ExecuteGitkForCurrentBranch(StepConfig), ref location));

            location.X += empty.X;
            this.panelStatus.Controls.Add(new ActionButton(toolTips, "Get latest", null, new Program.Action.ExecuteGetLatest(StepConfig), ref location));
            this.panelStatus.Controls.Add(new ActionButton(toolTips, "Merge", null, new Program.Action.ExecuteMerge(StepConfig), ref location));

            location.X += empty.X;
            this.panelStatus.Controls.Add(new ActionButton(toolTips, "Delete all changes", null, new Program.Action.ExecuteDeleteAllChanges(StepConfig), ref location));
            this.panelStatus.Controls.Add(new ActionButton(toolTips, "Repair options", null, new Program.Action.ExecuteGitRepair(StepConfig), ref location));

            //Second row
            location.X = lblStatusValue.Location.X;
            location.Y += 70;
            int count = 0;

            var exploreLogFiles = new Program.Action.StartExploreLogfiles(StepConfig);
            if (exploreLogFiles.isStartable())
            {
                this.panelStatus.Controls.Add(new ActionButton(toolTips, "Explore logfiles", null, exploreLogFiles, ref location));
                count++;
            }
            var openConfigFiles = new Program.Action.ExecuteOpenConfigfiles(StepConfig);
            if (openConfigFiles.isStartable())
            {
                this.panelStatus.Controls.Add(new ActionButton(toolTips, "Open configfiles", null, openConfigFiles, ref location));
                count++;
            }

            if (count>0) location.X += empty.X;
            count = 0;

            var clearCache = new Program.Action.ExecuteClearCache(StepConfig);
            if (clearCache.isStartable())
            {
                this.panelStatus.Controls.Add(new ActionButton(toolTips, "Clear cache", null, clearCache, ref location));
                count++;
            }

            var deleteLogFiles = new Program.Action.ExecuteDeleteLogfiles(StepConfig);
            if (deleteLogFiles.isStartable())
            {
                this.panelStatus.Controls.Add(new ActionButton(toolTips, "Delete logfiles", null, deleteLogFiles, ref location));
                count++;
            }

            //third row
            location.X = lblStatusValue.Location.X;
            location.Y += 70;

            var smtpserver = new Program.Action.StartSmtpServer(StepConfig);
            if (smtpserver.isStartable())
            {
                this.panelStatus.Controls.Add(new ActionButton(toolTips, "Smtp Server", null, smtpserver, ref location));
            }

            var fiddler = new Program.Action.StartFiddler(StepConfig);
            if (browser.isStartable() && fiddler.isStartable())
            {
                this.panelStatus.Controls.Add(new ActionButton(toolTips, "Fiddler", null, fiddler, ref location));
            }

            var ngrok = new Program.Action.ExecuteStartNgrok(StepConfig);
            if (browser.isStartable() && ngrok.isStartable())
            {
                this.panelStatus.Controls.Add(new ActionButton(toolTips, "Ngrok", null, ngrok, ref location));
            }
        }

        private void ViewHistoryOfAll(Program.Action.StepsExecutor.StepConfig StepConfig)
        {
            var gitk = new Program.Action.StartGitkAll(StepConfig);
            gitk.execute();
        }

        private void ViewHistoryOfFile(Program.Action.StepsExecutor.StepConfig StepConfig)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Show history of one file";
                openFileDialog.InitialDirectory = StepConfig.Config.ProjectWorkingDirectory.Path.ToString();
                openFileDialog.Filter = "All files (*.*)|*.*";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    var gitk = new Program.Action.StartGitk(StepConfig,
                        new List<string>() { "HEAD", StepConfig.Config.ProjectWorkingDirectory.Git.MainBranch },
                        null,
                        new List<string>() { openFileDialog.FileName });
                    gitk.execute();
                }
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

        private object _frontendLocked_LockObj = new Object();
        private volatile bool _frontendLocked = false;
        public void lockFrontend()
        {
            lock (_frontendLocked_LockObj)
            {
                if (_frontendLocked) throw new Exception("frontend is already locked");
                _frontendLocked = true;
            }

            if (_refresh != null)  _refresh.Suspend();
            showLocked();
        }

        public bool isFrontendLocked()
        {
            return _frontendLocked;
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
            if (_frontendLocked)
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
            lock (_frontendLocked_LockObj)
            {
                if (!_frontendLocked) throw new Exception("frontend is not locked");
                _frontendLocked = false;
            }

            _refresh.Resume();
            backToStatus();
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

        private void showErrorPanel(Program.Action.IVisualizerMessage message)
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

            Point location;
            TextBox input;
            switch (message.VisualizerMessageType)
            {
                case Program.Action.IVisualizerMessageType.options:
                    errorInput.Visible = false;

                    location = new Point(errorText.Left, errorText.Top + errorText.Height + 30);
                    input = null;
                    break;

                case Program.Action.IVisualizerMessageType.input:
                    location = new Point(errorText.Left, errorText.Top + errorText.Height + 5);
                    errorInput.Location = location;
                    errorInput.Width = errorText.MaximumSize.Width;
                    errorInput.Text = String.Empty;
                    errorInput.Visible = true;

                    location = new Point(errorInput.Left, errorInput.Top + errorInput.Height + 30);
                    input = errorInput;
                    break;

                default:
                    throw new Exception("Unknown IVisualizerMessageType");
            }

            {
                int no = 0;
                foreach (var item in message.VisualizerMessageButtons)
                {
                    ErrorButton button = _errorButtons[no];
                    button.ShowErrorButton(toolTips, input, item, ref location, size);

                    no++;
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

            if (message.VisualizerMessageType == Program.Action.IVisualizerMessageType.input)
            {
                errorInput.Focus();
            }
        }

        public void message(Program.Action.IVisualizerMessage message)
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
