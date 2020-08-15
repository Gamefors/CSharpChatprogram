using ClientGUI.Dir.Objects;
using System.Net.Sockets;
using System;
using System.Net;
using ClientGUI.Dir.ViewModel;
using System.Diagnostics;

namespace ClientGUI.Dir.Utils.CSocket
{
    class SocketClient
    {
        private MainWindowViewModel mainWindowViewModel;
        Action<bool> connectEvent;
        Action<string> PressEnterEvent;
        Action<string> loginEvent;
        private static Config config;
        public bool tryLogin = true;
        
        public Socket client;

        public SocketClient(Config config_, Action<bool> connectEvent_, Action<string> loginEvent_, Action<string> PressEnterEvent_, MainWindowViewModel mainWindowViewModel_)
        {
            mainWindowViewModel = mainWindowViewModel_;
            connectEvent = connectEvent_;
            PressEnterEvent = PressEnterEvent_;
            loginEvent = loginEvent_;
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
                Debug.WriteLine(e.ToString());
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
                connectEvent(true);
                client.EndConnect(ar);

                ServerHandler serverHandler = new ServerHandler(config, client, loginEvent, PressEnterEvent, mainWindowViewModel);

            }
            catch (Exception)
            {
                client = null;
                connectEvent(false);

            }
        }

    }
}
