using ClientGUI.Dir.Objects;
using ClientGUI.Dir.ViewModel;
using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ClientGUI.Dir.Utils.CSocket
{
    class ServerHandler
    {
        private MainWindowViewModel MainWindowViewModel;
        private ChatViewModel chatViewModel;
        private static Thread receiveThread;
        private Action<string> loginEvent;
        private Action<string> PressEnterEvent;
        private static Socket client;
        private Config config;
        private bool running = true;
        public ServerHandler(Config config_, Socket client_, Action<string> loginEvent_, Action<string> PressEnterEvent_, MainWindowViewModel MainWindowViewModel_)
        {
            MainWindowViewModel = MainWindowViewModel_;
            loginEvent = loginEvent_;
            PressEnterEvent = PressEnterEvent_;
            config = config_;
            client = client_;

            receiveThread = new Thread(new ThreadStart(Receive));
            receiveThread.Start();
            Send(client, "login:" + config.loginName + ":" + config.loginPassword + "<EOF>");

            PressEnterEvent += (string msg) =>
            {
                HandleInput(msg);
            };
            chatViewModel = new ChatViewModel(PressEnterEvent);
            while (running)
            {
                
            }
        }

        public void HandleInput(string input)
        {
            string[] args = input.Split(" ");
            string cmd = args[0];
            switch (cmd)
            {
                default:
                    Send(client, "chat:" + input + "<EOF>");
                    break;
            }

        }

        public void Receive()
        {
            ClientObject state = new ClientObject();
            state.workSocket = client;
            try
            {
                client.BeginReceive(state.buffer, 0, ClientObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                ClientObject state = (ClientObject)ar.AsyncState;
                Socket client = state.workSocket;

                int bytesRead = client.EndReceive(ar);
                HandleResponse(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));
                client.BeginReceive(state.buffer, 0, ClientObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
        }

        private void HandleResponse(string response)
        {
            if (response.Contains("WRONGCREDENTIALS"))
            {
                loginEvent("WC");
            }else if (response.Contains("ALREADYLOGGEDIN"))
            {
                loginEvent("AL");
            } else if (response.Contains("SUCCESFULLYLOGGEDIN"))
            {
                loginEvent("SL");
                MainWindowViewModel.setActiveViewMode(chatViewModel); 
            }
            else
            {
                chatViewModel.writeOutput(response);
            }
            
        }

        public void Send(Socket client, String data)
        {
            byte[] byteData = Encoding.ASCII.GetBytes(data);
            client.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), client);
        }

        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                Socket client = (Socket)ar.AsyncState;
                int bytesSent = client.EndSend(ar);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
        }

    }
}
