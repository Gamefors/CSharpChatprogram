using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Server
{
    class SocketServer
    {
        private static Config config;
        private static LogHelper logHelper;
        private static MysqlHelper mysqlHelper;

        public List<Client> clients = new List<Client>();
        public List<Thread> clientThreads = new List<Thread>();

        private static bool running = true;

        public SocketServer(Config config_, LogHelper logHelper_, MysqlHelper mysqlHelper_)
        {
            config = config_;
            logHelper = logHelper_;
            mysqlHelper = mysqlHelper_;
        }

        public void Start()
        {
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, config.socketPort);

            Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(config.socketMaxConnectionQueue);
                logHelper.Log("Server started.", LogType.Info);
                while (running)
                {
                    listener.BeginAccept(new AsyncCallback(StartClientHandler), listener);
                    //3 seconds in between accepting clients
                    Thread.Sleep((1000 * 3)); 
                }
                foreach (Client client in clients)
                {
                    client.connected = false;
                    if (client.Disconnect())
                    {
                        logHelper.Log("Succesfully disconnected: " + client.name + ".", LogType.Info);
                    }
                    else
                    {
                        logHelper.Log("Error disconnecting: " + client.name + ".", LogType.Error);
                    }
                }
                
                logHelper.Log("Server stopped.", LogType.Info);
                Console.WriteLine("press any key to close console.");
                Console.Read();
            }
            catch (Exception ex)
            {
                logHelper.Log("Exception while listening for new client. Exeption: ", LogType.Error);
                logHelper.Log(ex.ToString(), LogType.Error);
            }
        }

        public void StartClientHandler(IAsyncResult ar)
        {
            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            Client client = new Client();
            client.socket = handler;
            clients.Add(client);

            ClientHandler clientHandler = new ClientHandler(logHelper, mysqlHelper, client, clients);
            Thread clientThread = new Thread(new ThreadStart(clientHandler.Start));
            clientThreads.Add(clientThread);
            clientThread.Start();
        }

        public void Stop()
        {
            logHelper.Log("Server is shuting down...", LogType.Info);
            running = false;
        }

    }
}
