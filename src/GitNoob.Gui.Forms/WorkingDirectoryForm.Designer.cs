namespace GitNoob.Gui.Forms
{
    partial class WorkingDirectoryForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WorkingDirectoryForm));
            this.lblMainbranch = new System.Windows.Forms.Label();
            this.lblCurrentbranch = new System.Windows.Forms.Label();
            this.lblMainbranchValue = new System.Windows.Forms.Label();
            this.lblWorkingdirectory = new System.Windows.Forms.Label();
            this.lblWorkingdirectoryValue = new System.Windows.Forms.Label();
            this.lblCommitname = new System.Windows.Forms.Label();
            this.lblCommitnameValue = new System.Windows.Forms.Label();
            this.Picture = new System.Windows.Forms.PictureBox();
            this.panelStatus = new System.Windows.Forms.Panel();
            this.lblStatusValue = new System.Windows.Forms.Label();
            this.labelBusy = new System.Windows.Forms.Label();
            this.panelError = new System.Windows.Forms.Panel();
            this.errorInput2 = new System.Windows.Forms.TextBox();
            this.errorInput = new System.Windows.Forms.TextBox();
            this.errorBottom = new System.Windows.Forms.Label();
            this.errorPicture = new System.Windows.Forms.PictureBox();
            this.errorText = new System.Windows.Forms.LinkLabel();
            this.lblCurrentbranchValue = new System.Windows.Forms.LinkLabel();
            this.toolTips = new System.Windows.Forms.ToolTip(this.components);
            this.errorText2 = new System.Windows.Forms.LinkLabel();
            ((System.ComponentModel.ISupportInitialize)(this.Picture)).BeginInit();
            this.panelStatus.SuspendLayout();
            this.panelError.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorPicture)).BeginInit();
            this.SuspendLayout();
            // 
            // lblMainbranch
            // 
            this.lblMainbranch.AutoSize = true;
            this.lblMainbranch.Location = new System.Drawing.Point(368, 13);
            this.lblMainbranch.Name = "lblMainbranch";
            this.lblMainbranch.Size = new System.Drawing.Size(86, 19);
            this.lblMainbranch.TabIndex = 0;
            this.lblMainbranch.Text = "Main branch";
            // 
            // lblCurrentbranch
            // 
            this.lblCurrentbranch.AutoSize = true;
            this.lblCurrentbranch.Location = new System.Drawing.Point(368, 68);
            this.lblCurrentbranch.Name = "lblCurrentbranch";
            this.lblCurrentbranch.Size = new System.Drawing.Size(102, 19);
            this.lblCurrentbranch.TabIndex = 4;
            this.lblCurrentbranch.Text = "Current branch";
            // 
            // lblMainbranchValue
            // 
            this.lblMainbranchValue.AutoSize = true;
            this.lblMainbranchValue.Location = new System.Drawing.Point(507, 13);
            this.lblMainbranchValue.Name = "lblMainbranchValue";
            this.lblMainbranchValue.Size = new System.Drawing.Size(18, 19);
            this.lblMainbranchValue.TabIndex = 1;
            this.lblMainbranchValue.Text = "...";
            // 
            // lblWorkingdirectory
            // 
            this.lblWorkingdirectory.AutoSize = true;
            this.lblWorkingdirectory.Location = new System.Drawing.Point(368, 40);
            this.lblWorkingdirectory.Name = "lblWorkingdirectory";
            this.lblWorkingdirectory.Size = new System.Drawing.Size(118, 19);
            this.lblWorkingdirectory.TabIndex = 2;
            this.lblWorkingdirectory.Text = "Working directory";
            // 
            // lblWorkingdirectoryValue
            // 
            this.lblWorkingdirectoryValue.AutoSize = true;
            this.lblWorkingdirectoryValue.Location = new System.Drawing.Point(507, 40);
            this.lblWorkingdirectoryValue.Name = "lblWorkingdirectoryValue";
            this.lblWorkingdirectoryValue.Size = new System.Drawing.Size(18, 19);
            this.lblWorkingdirectoryValue.TabIndex = 3;
            this.lblWorkingdirectoryValue.Text = "...";
            // 
            // lblCommitname
            // 
            this.lblCommitname.AutoSize = true;
            this.lblCommitname.Location = new System.Drawing.Point(368, 96);
            this.lblCommitname.Name = "lblCommitname";
            this.lblCommitname.Size = new System.Drawing.Size(96, 19);
            this.lblCommitname.TabIndex = 6;
            this.lblCommitname.Text = "Commit name";
            // 
            // lblCommitnameValue
            // 
            this.lblCommitnameValue.AutoSize = true;
            this.lblCommitnameValue.Location = new System.Drawing.Point(507, 96);
            this.lblCommitnameValue.Name = "lblCommitnameValue";
            this.lblCommitnameValue.Size = new System.Drawing.Size(18, 19);
            this.lblCommitnameValue.TabIndex = 7;
            this.lblCommitnameValue.Text = "...";
            // 
            // Picture
            // 
            this.Picture.Location = new System.Drawing.Point(10, 13);
            this.Picture.Name = "Picture";
            this.Picture.Size = new System.Drawing.Size(336, 102);
            this.Picture.TabIndex = 11;
            this.Picture.TabStop = false;
            // 
            // panelStatus
            // 
            this.panelStatus.Controls.Add(this.lblStatusValue);
            this.panelStatus.Location = new System.Drawing.Point(0, 121);
            this.panelStatus.Margin = new System.Windows.Forms.Padding(0);
            this.panelStatus.Name = "panelStatus";
            this.panelStatus.Size = new System.Drawing.Size(982, 265);
            this.panelStatus.TabIndex = 12;
            // 
            // lblStatusValue
            // 
            this.lblStatusValue.AutoSize = true;
            this.lblStatusValue.Location = new System.Drawing.Point(10, 13);
            this.lblStatusValue.Name = "lblStatusValue";
            this.lblStatusValue.Size = new System.Drawing.Size(95, 19);
            this.lblStatusValue.TabIndex = 9;
            this.lblStatusValue.Text = "no changes, ...";
            // 
            // labelBusy
            // 
            this.labelBusy.AutoSize = true;
            this.labelBusy.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.labelBusy.Location = new System.Drawing.Point(898, 27);
            this.labelBusy.Name = "labelBusy";
            this.labelBusy.Size = new System.Drawing.Size(76, 20);
            this.labelBusy.TabIndex = 13;
            this.labelBusy.Text = "labelBusy";
            // 
            // panelError
            // 
            this.panelError.Controls.Add(this.errorText2);
            this.panelError.Controls.Add(this.errorInput2);
            this.panelError.Controls.Add(this.errorInput);
            this.panelError.Controls.Add(this.errorBottom);
            this.panelError.Controls.Add(this.errorPicture);
            this.panelError.Controls.Add(this.errorText);
            this.panelError.Location = new System.Drawing.Point(0, 121);
            this.panelError.Margin = new System.Windows.Forms.Padding(0);
            this.panelError.Name = "panelError";
            this.panelError.Size = new System.Drawing.Size(982, 265);
            this.panelError.TabIndex = 16;
            // 
            // errorInput2
            // 
            this.errorInput2.Location = new System.Drawing.Point(76, 189);
            this.errorInput2.Name = "errorInput2";
            this.errorInput2.Size = new System.Drawing.Size(100, 25);
            this.errorInput2.TabIndex = 7;
            // 
            // errorInput
            // 
            this.errorInput.Location = new System.Drawing.Point(76, 44);
            this.errorInput.Name = "errorInput";
            this.errorInput.Size = new System.Drawing.Size(100, 25);
            this.errorInput.TabIndex = 6;
            // 
            // errorBottom
            // 
            this.errorBottom.AutoSize = true;
            this.errorBottom.Location = new System.Drawing.Point(898, 246);
            this.errorBottom.Name = "errorBottom";
            this.errorBottom.Size = new System.Drawing.Size(85, 19);
            this.errorBottom.TabIndex = 4;
            this.errorBottom.Text = "errorBottom";
            // 
            // errorPicture
            // 
            this.errorPicture.Location = new System.Drawing.Point(12, 12);
            this.errorPicture.Margin = new System.Windows.Forms.Padding(0);
            this.errorPicture.Name = "errorPicture";
            this.errorPicture.Size = new System.Drawing.Size(48, 48);
            this.errorPicture.TabIndex = 3;
            this.errorPicture.TabStop = false;
            // 
            // errorText
            // 
            this.errorText.Location = new System.Drawing.Point(72, 12);
            this.errorText.Name = "errorText";
            this.errorText.Size = new System.Drawing.Size(898, 95);
            this.errorText.TabIndex = 5;
            this.errorText.TabStop = true;
            this.errorText.Text = "errorText";
            this.errorText.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.errorText_LinkClicked);
            // 
            // lblCurrentbranchValue
            // 
            this.lblCurrentbranchValue.AutoSize = true;
            this.lblCurrentbranchValue.Location = new System.Drawing.Point(507, 68);
            this.lblCurrentbranchValue.Name = "lblCurrentbranchValue";
            this.lblCurrentbranchValue.Size = new System.Drawing.Size(18, 19);
            this.lblCurrentbranchValue.TabIndex = 17;
            this.lblCurrentbranchValue.TabStop = true;
            this.lblCurrentbranchValue.Text = "...";
            this.lblCurrentbranchValue.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lblCurrentbranchValue_LinkClicked);
            // 
            // errorText2
            // 
            this.errorText2.Location = new System.Drawing.Point(72, 119);
            this.errorText2.Name = "errorText2";
            this.errorText2.Size = new System.Drawing.Size(898, 95);
            this.errorText2.TabIndex = 8;
            this.errorText2.TabStop = true;
            this.errorText2.Text = "errorText2";
            // 
            // WorkingDirectoryForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(982, 382);
            this.Controls.Add(this.lblCurrentbranchValue);
            this.Controls.Add(this.labelBusy);
            this.Controls.Add(this.Picture);
            this.Controls.Add(this.lblCommitnameValue);
            this.Controls.Add(this.lblCommitname);
            this.Controls.Add(this.lblWorkingdirectoryValue);
            this.Controls.Add(this.lblWorkingdirectory);
            this.Controls.Add(this.lblMainbranchValue);
            this.Controls.Add(this.lblCurrentbranch);
            this.Controls.Add(this.lblMainbranch);
            this.Controls.Add(this.panelError);
            this.Controls.Add(this.panelStatus);
            this.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "WorkingDirectoryForm";
            this.Text = "GitNoob - working directory";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.WorkingDirectoryForm_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.Picture)).EndInit();
            this.panelStatus.ResumeLayout(false);
            this.panelStatus.PerformLayout();
            this.panelError.ResumeLayout(false);
            this.panelError.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorPicture)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblMainbranch;
        private System.Windows.Forms.Label lblCurrentbranch;
        private System.Windows.Forms.Label lblMainbranchValue;
        private System.Windows.Forms.Label lblWorkingdirectory;
        private System.Windows.Forms.Label lblWorkingdirectoryValue;
        private System.Windows.Forms.Label lblCommitname;
        private System.Windows.Forms.Label lblCommitnameValue;
        private System.Windows.Forms.PictureBox Picture;
        private System.Windows.Forms.Panel panelStatus;
        private System.Windows.Forms.Label lblStatusValue;
        private System.Windows.Forms.Label labelBusy;
        private System.Windows.Forms.Panel panelError;
        private System.Windows.Forms.PictureBox errorPicture;
        private System.Windows.Forms.Label errorBottom;
        private System.Windows.Forms.LinkLabel errorText;
        private System.Windows.Forms.TextBox errorInput;
        private System.Windows.Forms.LinkLabel lblCurrentbranchValue;
        private System.Windows.Forms.ToolTip toolTips;
        private System.Windows.Forms.TextBox errorInput2;
        private System.Windows.Forms.LinkLabel errorText2;
    }
}

