using System;
using System.Collections.Generic;
using System.Threading;

namespace Server
{
    class Server
    {
        public static bool running = true;

        private static Config config;
        private static LogHelper logHelper;
        private static MysqlHelper mysqlHelper;
        private static SocketServer socketServer;
        
        private static Thread socketServerThread;

        static void Main()
        {
            config = new Config();
            logHelper = new LogHelper();
            mysqlHelper = new MysqlHelper(logHelper);
            socketServer = new SocketServer(config, logHelper, mysqlHelper);

            socketServerThread = new Thread(new ThreadStart(socketServer.Start));

            logHelper.Log("Server is starting...", LogType.Info);

            if (!mysqlHelper.Connect(config.mysqlIp, config.mysqlUsername, config.mysqlPassword, config.mysqlDatabase))
                Console.WriteLine("TODO:use sqllite fallback"); //TODO:use sqllite fallback
           
            socketServerThread.Start();
           
            while (running)
            {
                string input = Console.ReadLine();
                HandleInput(input);
            }
            socketServer.Stop();
        }

        private static void HandleInput(string input)
        {
            string[] args = input.Split(" ");
            string cmd = args[0];
            switch (cmd)
            {
                case "clients":
                    List<Client> clients = socketServer.clients;
                    if (clients.Count != 0)
                    {
                        Console.WriteLine("===Clients===");
                        foreach (Client client in socketServer.clients)
                        {
                            Console.WriteLine("-" + client.name);
                        }
                    }
                    else
                    {
                        Console.WriteLine("No clients connected.");
                    }
                    break;
                case "stop":
                    running = false;
                    break;
                default:
                    Console.WriteLine("Command not found.");
                    break;
            }

        }

    }
}
