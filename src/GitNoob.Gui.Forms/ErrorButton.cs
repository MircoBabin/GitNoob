using GitNoob.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace GitNoob.Gui.Forms
{
    class ErrorButton : Button
    {
        private System.Action<Visualizer.VisualizerInput> _action = null;
        private TextBox _input = null;
        private TextBox _input2 = null;
        private ControlCollection _parent = null;

        private new bool Visible { get { return base.Visible; } } // because of subbuttons, use ShowErrorbutton() & HideErrorButton()

        public ErrorButton(ControlCollection parent) : base()
        {
            this.Click += ErrorButton_Click;
            base.Visible = false;

            _parent = parent;
            _parent.Add(this);
        }

        private List<Button> subbuttons = new List<Button>();
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                foreach (var subbutton in subbuttons)
                {
                    DisposeSubbutton(subbutton);
                }
            }

            subbuttons.Clear();
            _parent = null;
        }

        private void ErrorButton_Click(object sender, EventArgs e)
        {
            ClickExecuteAction(_action);
        }

        private void ClickExecuteAction(System.Action<Visualizer.VisualizerInput> action)
        {
            if (action == null) return;

            Visualizer.VisualizerInput input = new Visualizer.VisualizerInput();
            if (_input != null)
            {
                input.inputValue = _input.Text;
            }
            if (_input2 != null)
            {
                input.input2Value = _input2.Text;
            }

            action(input);
        }

        public void HideErrorButton()
        {
            base.Visible = false;

            foreach (var subbutton in subbuttons)
            {
                subbutton.Visible = false;
                DisposeSubbutton(subbutton);
            }
            subbuttons.Clear();
        }

        private class SubbuttonProperties
        {
            public ToolTip toolTips { get; set; }

            public SubbuttonProperties(ToolTip toolTips)
            {
                this.toolTips = toolTips;
            }
        }

        private void DisposeSubbutton(Button subbutton)
        {
            try
            {
                _parent.Remove(subbutton);
                if (subbutton.Tag != null)
                {
                    SubbuttonProperties props = (SubbuttonProperties)subbutton.Tag;

                    if (props.toolTips != null)
                    {
                        // https://docs.microsoft.com/en-us/troubleshoot/developer/dotnet/framework/general/winform-app-use-settooltip-not-release-memory
                        // When calling ToolTip.SetToolTip to associate a ToolTip with a Windows Forms control, 
                        // the ToolTip object stores internal information, including a reference to the control.
                        //
                        // You can disassociate a particular Windows Forms control from the ToolTip by calling ToolTip.SetToolTip 
                        // and passing a reference to the control and an empty string for the ToolTip caption. When an empty string is 
                        // passed for the ToolTip caption, the SetToolTip method removes the reference to the Windows Forms control from
                        // internal information.

                        props.toolTips.SetToolTip(subbutton, "");
                        props.toolTips = null;
                    }

                    subbutton.Tag = null;
                }
                subbutton.Dispose();
            }
            catch { }
        }

        public void ShowErrorButton(ToolTip toolTips, TextBox Input, TextBox Input2, Visualizer.VisualizerMessageButton button, ref Point Location, Size size)
        {
            HideErrorButton();

            _action = button.onClicked;
            _input = Input;
            _input2 = Input2;

            const int margin = 8;
            Point sublocation = new Point(Location.X, Location.Y);
            int subsize = 0;
            foreach (var sub in button.subButtons)
            {
                var subbutton = new Button();
                subbutton.ClientSize = new Size(32, 32);
                subbutton.Location = new Point(sublocation.X, sublocation.Y);
                sublocation.X += 32 + margin;
                subsize += 32 + margin;

                try
                {
                    subbutton.Image = ImageUtils.IconToBitmapOfSize(sub.icon, 32, 32, Color.Transparent);
                }
                catch { }

                if (toolTips != null)
                {
                    toolTips.SetToolTip(subbutton, sub.hint);
                }
                subbutton.Tag = new SubbuttonProperties(toolTips);

                subbutton.Click += (sender, e) =>
                {
                    ClickExecuteAction(sub.onClicked);
                };

                _parent.Add(subbutton);

                subbuttons.Add(subbutton);
            }

            size.Width -= subsize;
            SetErrorButton(button.text, ref Location, size);
            this.Location = new Point(this.Location.X + subsize, this.Location.Y);
            if (button.subButtons.Count > 0)
            {
                if (this.ClientSize.Height < 32)
                {
                    Size newsize = new Size(this.ClientSize.Width, 32);
                    this.AutoSize = false;
                    this.ClientSize = newsize;
                }
                else
                {
                    foreach (var subbutton in subbuttons)
                    {
                        subbutton.ClientSize = new Size(32, this.ClientSize.Height);
                    }
                }
            }
        }

        private void SetErrorButton(string Description, ref Point Location, Size size)
        { 
            this.MaximumSize = size;
            this.MinimumSize = size;
            this.AutoSize = true;
            this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.TextAlign = ContentAlignment.MiddleLeft;

            this.Text = Description.Replace("&", "&&");
            this.Location = new Point(Location.X, Location.Y);
            base.Visible = true;

            Location.Y += this.Height + 5;
        }
    }
}
