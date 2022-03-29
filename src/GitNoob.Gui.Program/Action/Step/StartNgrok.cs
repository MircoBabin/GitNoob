using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace GitNoob.Gui.Program.Action.Step
{
    public class StartNgrok : Step
    {
        private string _ngrokExe;
        private string _ngrokUrl;
        public StartNgrok(string ngrokExe, string ngrokUrl) : base()
        {
            _ngrokExe = ngrokExe;
            _ngrokUrl = ngrokUrl;
            if (!_ngrokUrl.EndsWith("/")) _ngrokUrl += "/";
        }

        //https://ngrok.com/docs#list-tunnels
        [DataContract]
        private class Tunnels
        {
            [DataMember]
            public Tunneldetail[] tunnels = null;
        }

        //https://ngrok.com/docs#tunnel-detail
        [DataContract]
        private class Tunneldetail
        {
            [DataMember]
            public string name = null;

            [DataMember]
            public string uri = null;

            [DataMember]
            public string public_url = null;

            [DataMember]
            public string proto = null;

            [DataMember]
            public Tunneldetail_config config = null;
        }

        [DataContract]
        private class Tunneldetail_config
        {
            [DataMember]
            public string addr = null;

            [DataMember]
            public bool inspect = false;
        }

        protected override bool run()
        {
            BusyMessage = "Busy - starting Ngrok";

            var running = false;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_ngrokUrl + "api/tunnels");
                request.Method = "GET";
                var response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Tunnels list = new Tunnels();
                    using (Stream stream = response.GetResponseStream())
                    {
                        var ser = new DataContractJsonSerializer(list.GetType());
                        list = ser.ReadObject(stream) as Tunnels;
                    }

                    if (list.tunnels.Length > 0)
                    {
                        running = true;
                    }
                }
            }
            catch { }

            if (!running)
            {
                var info = new System.Diagnostics.ProcessStartInfo
                {
                    WorkingDirectory = StepsExecutor.Config.ProjectWorkingDirectory.Path.ToString(),
                    FileName = _ngrokExe,
                    Arguments = "http -host-header=localhost:" + StepsExecutor.Config.ProjectWorkingDirectory.Apache.Port + " localhost:" + StepsExecutor.Config.ProjectWorkingDirectory.Apache.Port,
                    UseShellExecute = false,
                };
                System.Diagnostics.Process.Start(info);
            }

            return true;
        }
    }
}
