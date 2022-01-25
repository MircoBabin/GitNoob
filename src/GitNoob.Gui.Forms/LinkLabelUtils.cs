using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace GitNoob.Gui.Forms
{
    public class LinkLabelUtils
    {
        private class MyLink
        {
            public string LinkName { get; set; }
            public int Start { get; set; }
            public string Description { get; set; }
            public System.Action Action { get; set; }
        }

        public static void SetLinkLabel(LinkLabel label, string text, Dictionary<string, System.Action> actions, string appendText = null)
        {
            //"This is a text with a [link test]link in it[/link]."
            //- "test" is the name of the [link]
            //- actions["test"] should exist, will be called if link is pressed

            const string beginmarkerStart = "[link ";
            const string beginmarkerEnd = "]";
            const string endmarker = "[/link]";

            label.Links.Clear();
            var mylinks = new List<MyLink>();

            int from = 0;
            StringBuilder labeltext = new StringBuilder();
            while(from < text.Length)
            {
                //[link
                int start = text.IndexOf(beginmarkerStart, from);
                if (start < 0)
                {
                    labeltext.Append(text.Substring(from));
                    break;
                }

                labeltext.Append(text.Substring(from, start - from));
                start += beginmarkerStart.Length;

                //]
                int end = text.IndexOf(beginmarkerEnd, start);
                if (end < start) break;

                var link = new MyLink();
                link.LinkName = text.Substring(start, end - start);

                end += beginmarkerEnd.Length;

                //[/link]
                int end1 = text.IndexOf(endmarker, end);
                if (end1 < end)
                {
                    labeltext.Append(text.Substring(end));
                    break;
                }

                //label.Links
                link.Start = labeltext.Length;
                link.Description = text.Substring(end, end1 - end);

                System.Action action;
                if (actions != null && actions.TryGetValue(link.LinkName, out action))
                {
                    link.Action = action;
                    mylinks.Add(link);
                }

                end1 += endmarker.Length;
                labeltext.Append(link.Description);

                //Next
                from = end1;
            }

            if (!string.IsNullOrWhiteSpace(appendText)) labeltext.Append(appendText);
            label.Text = labeltext.ToString();

            foreach(var link in mylinks)
            {
                label.Links.Add(link.Start, link.Description.Length, link);
            }
        }

        public static void ExecuteLinkClicked(LinkLabelLinkClickedEventArgs e)
        {
            MyLink link = e.Link.LinkData as MyLink;
            if (link == null) return;

            link.Action();
        }
    }
}
