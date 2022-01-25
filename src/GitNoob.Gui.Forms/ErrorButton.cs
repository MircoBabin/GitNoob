using System;
using System.Drawing;
using System.Windows.Forms;

namespace GitNoob.Gui.Forms
{
    class ErrorButton : Button
    {
        private System.Action<Program.Action.MessageInput> _action = null;
        private TextBox _input = null;

        public ErrorButton() : base()
        {
            this.Click += ErrorButton_Click;
        }

        private void ErrorButton_Click(object sender, EventArgs e)
        {
            if (_action != null)
            {
                Program.Action.MessageInput input = new Program.Action.MessageInput();

                if (_input != null)
                {
                    input.inputValue = _input.Text;
                }

                _action(input);
            }
        }

        public void ShowErrorButton(TextBox Input, string Description, System.Action<Program.Action.MessageInput> Action, ref Point Location, Size size)
        {
            _action = Action;
            _input = Input;

            SetErrorButton(Description, ref Location, size);
        }

        private void SetErrorButton(string Description, ref Point Location, Size size)
        { 
            this.MaximumSize = size;
            this.MinimumSize = size;
            this.AutoSize = true;
            this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.TextAlign = ContentAlignment.MiddleLeft;

            this.Text = Description;
            this.Location = new Point(Location.X, Location.Y);
            this.Visible = true;

            Location.Y += this.Height + 5;
        }
    }
}
