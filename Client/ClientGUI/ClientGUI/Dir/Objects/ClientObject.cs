using System.Net.Sockets;
using System.Text;

namespace ClientGUI.Dir.Objects
{
    class ClientObject
    {
        public Socket workSocket = null;
        public const int BufferSize = 256;
        public byte[] buffer = new byte[BufferSize];
        public StringBuilder sb = new StringBuilder();
    }
}
