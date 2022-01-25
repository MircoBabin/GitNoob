namespace GitNoob.Config
{
    public class Ngrok
    {
        public string NgrokPath { get; set; }

        public int Port { get; set; }

        public Ngrok()
        {
            NgrokPath = string.Empty;
            Port = 4040; //default NGROK port
        }

        public void CopyFrom(Ngrok other)
        {
            NgrokPath = other.NgrokPath;
            Port = other.Port;
        }
    }
}
