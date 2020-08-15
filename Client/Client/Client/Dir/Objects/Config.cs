using System.Net;

namespace Client
{
    class Config
    {
        public string serverIp = Dns.GetHostName();
        public int serverPort = 11000;


        public string loginName = "";
        public string loginPassword = "";
    }
}
