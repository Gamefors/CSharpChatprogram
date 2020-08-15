using System;
using System.Net;
using System.Net.Sockets;

namespace Client
{
    class SocketClient
    {
        private static Config config;

        public bool tryLogin = true;

        public Socket client;

        public SocketClient(Config config_)
        {
            config = config_;
        }

        public void Start()
        {
            try
            {
                IPHostEntry ipHostInfo = Dns.GetHostEntry(config.serverIp);
                IPAddress ipAddress = ipHostInfo.AddressList[0];
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, config.serverPort);

                client = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                client.BeginConnect(remoteEP, new AsyncCallback(ConnectCallback), client);
               
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public void Disconnect()
        {
            client.Shutdown(SocketShutdown.Both);
            client.Close();
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                Socket client = (Socket)ar.AsyncState;

                client.EndConnect(ar);

                Console.WriteLine("Connection to server succesfully established.");

                ServerHandler serverHandler = new ServerHandler(config, client);

               

            }
            catch (Exception)
            {
                Console.WriteLine("Connection to server could not be established.");
                client = null;
                
            }
        }

    }
}
