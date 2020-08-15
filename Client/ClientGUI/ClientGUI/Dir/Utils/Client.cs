using ClientGUI.Dir.Objects;
using ClientGUI.Dir.Utils.CSocket;
using ClientGUI.Dir.ViewModel;
using System;
using System.Threading;

namespace ClientGUI.Dir.Utils
{
    class SClient
    {
        private static Config config;
        private static SocketClient socketClient;
        private static Thread socketClientThread;

        public SClient(Action<bool> connectEvent, Action<string> loginEvent, Action<string> PressEnterEvent, MainWindowViewModel mainWindowViewModel_)
        {
            config = new Config();
            socketClient = new SocketClient(config, connectEvent, loginEvent, PressEnterEvent, mainWindowViewModel_);
            socketClientThread = new Thread(new ThreadStart(socketClient.Start));
        }

        public void Login(string username, string password)
        {
            config.loginName = username;
            config.loginPassword = password;
            socketClientThread.Start();
        }

    }
}
