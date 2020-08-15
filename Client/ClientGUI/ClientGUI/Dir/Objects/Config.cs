using System.Net;

namespace ClientGUI.Dir.Objects
{
    class Config
    {
        public string serverIp = Dns.GetHostName();
        public int serverPort = 11000;


        public string loginName = "";
        public string loginPassword = "";
    }
}
