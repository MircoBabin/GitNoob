using System;
using System.Drawing;
using System.Windows.Forms;

namespace GitNoob.Gui.Forms
{
    public class ActionButton : Button
    {
        private Gui.Program.Action.IAction _action;

        public ActionButton(ToolTip toolTips, string text, Gui.Program.Action.IAction action, ref Point location) : base()
        {
            const int margin = 8;

            _action = action;

            this.ClientSize = new Size(48, 48);
            this.AutoSize = true;
            if (_action != null)
            {
                try
                {
                    this.Image = Program.Utils.ImageUtils.IconToBitmapOfSize(_action.icon(), 48, 48, Color.Transparent);
                }
                catch { }

                if (toolTips != null)
                {
                    toolTips.SetToolTip(this, text);
                }

                this.Click += ActionButton_Click;
            }

            this.Location = location;
            location.X = this.Left + this.Width + margin;
        }

        private void ActionButton_Click(object sender, EventArgs e)
        {
            _action.execute();
        }
    }
}
