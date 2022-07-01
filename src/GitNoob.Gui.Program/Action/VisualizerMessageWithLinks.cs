using System.Collections.Generic;
using System.Text;

namespace GitNoob.Gui.Program.Action
{
    public class VisualizerMessageWithLinks
    {
        public StringBuilder Message { get; set; }
        public Dictionary<string, System.Action> Links { get; set; }

        private int _linkno = 0;

        public VisualizerMessageWithLinks()
        {
            this.Message = new StringBuilder();

            this.Links = new Dictionary<string, System.Action>();
        }

        public VisualizerMessageWithLinks(string Message)
        {
            this.Message = new StringBuilder(Message);

            this.Links = new Dictionary<string, System.Action>();
        }

        public VisualizerMessageWithLinks(VisualizerMessageWithLinks Message)
        {
            this.Message = new StringBuilder(Message.Message.ToString().TrimEnd());

            this._linkno = Message._linkno;
            this.Links = new Dictionary<string, System.Action>();
            foreach(var item in Message.Links)
            {
                this.Links.Add(item.Key, item.Value);
            }
        }

        public void Append(string text)
        {
            Message.Append(text);
        }

        public void AppendLink(string text, System.Action execute)
        {
            _linkno++;
            string name = "link" + _linkno;

            Message.Append("[link ");
            Message.Append(name);
            Message.Append("]");

            Message.Append(text);

            Message.Append("[/link]");

            Links.Add(name, execute);
        }
    }
}
