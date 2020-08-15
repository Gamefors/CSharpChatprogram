using System.Net.Sockets;
using System.Text;

namespace Server
{
    class Client
    {
        //TODO move mysql here ? so sync is more easy
        public bool connected = true;
        public string name = null;
        public bool loggedIn = false;
        public Socket socket = null;
        public const int BufferSize = 1024;
        public byte[] buffer = new byte[BufferSize];
        public StringBuilder sb = new StringBuilder();
      
        public bool Disconnect()
        {
            if (socket != null)
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
                return true;
            }
            return false;
        }
    }
}
