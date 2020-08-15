using System;
using System.Threading;

namespace Client
{
    class Client
    {
        private static Config config;
        private static SocketClient socketClient;
        private static Thread socketClientThread;

        private static bool loggedIn = false;

        static void Main()
        {
            config = new Config();

            socketClient = new SocketClient(config);
            socketClientThread = new Thread(new ThreadStart(socketClient.Start));
            while (true)
            {
                if (!loggedIn)
                {
                    string input = Console.ReadLine();
                    HandleInput(input);
                }
            }
        }

        private static void HandleInput(string input)
        {
            string[] args = input.Split(" ");
            string cmd = args[0];
            switch (cmd)
            {
                case "login":
                    
                    if (!loggedIn)
                    {
                        loggedIn = true;
                        config.loginName = args[1];
                        config.loginPassword = args[2];
                        socketClientThread.Start();
                    }
                    
                    break;
            }

        }
    }
}
