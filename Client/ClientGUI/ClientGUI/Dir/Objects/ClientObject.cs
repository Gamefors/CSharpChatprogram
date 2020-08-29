using System.Net.Sockets;
using System.Text;

namespace ClientGUI.Dir.Objects
{
    class ClientObject
    {
        public bool myself;
        public string username { get; set; }
        public string rank { get; set; }
        public string RankColor { 
            get
            {
                switch (rank)
                {
                    case "User":
                        return "White";
                    case "Admin":
                        return "Red";
                    default:
                        return "White";
                }
            }
            set { RankColor = value; }
        }
        public Socket workSocket = null;
        public const int BufferSize = 256;
        public byte[] buffer = new byte[BufferSize];
        public StringBuilder sb = new StringBuilder();
    }
}
