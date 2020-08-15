using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    class ClientHandler
    {
        private static LogHelper logHelper;
        private static MysqlHelper mysqlHelper;

        private static List<Client> clients = new List<Client>();
        private static Client client;
        public ClientHandler(LogHelper logHelper_, MysqlHelper mysqlHelper_, Client client_, List<Client> clients_)
        {
            logHelper = logHelper_;
            mysqlHelper = mysqlHelper_;

            client = client_;
            clients = clients_;
        }
        
        public void Start()
        {
            client.socket.BeginReceive(client.buffer, 0, Client.BufferSize, 0, new AsyncCallback(ReceiveCallback), client);
        }

        public static void ReceiveCallback(IAsyncResult ar)
        {
            Client client = (Client)ar.AsyncState;
            Socket handler = client.socket;
            try
            {
                if (client.connected)
                {
                    int bytesRead = handler.EndReceive(ar);
                    if (bytesRead > 0)
                    {
                        client.sb.Append(Encoding.ASCII.GetString(client.buffer, 0, bytesRead));
                        string content = client.sb.ToString();
                        if (content.IndexOf("<EOF>") > -1)
                        {
                            String[] data = content.Remove(content.Length - 5).Split(":");
                            handleRequest(client, data);
                        }
                        else
                        {
                            if (client.connected)
                                handler.BeginReceive(client.buffer, 0, Client.BufferSize, 0, new AsyncCallback(ReceiveCallback), client);
                        }
                    }
                }
            }
            catch (SocketException)
            {
                logHelper.Log(client.name + " disconnected ungracefully.", LogType.Info);
                mysqlHelper.SetClientLoginStatus(client, false);
                clients.Remove(client);
            }
        }

        private static void handleRequest(Client client, String[] data)
        {
            String request = data[0];
            switch (request)
            {
                case "chat":
                    if (client.loggedIn)
                    {
                        String msg = data[1];
                        logHelper.Log("[" + client.name + "]: " + msg, LogType.Info);
                        foreach (Client client_ in clients)
                        {
                            if (!client_.name.Equals(client.name))
                                Send(client_.socket, "[" + client.name + "]: " + msg);
                        }
                    }
                    break;
                case "login":
                    client.name = data[1];
                    Login(client, data[2]);
                    break;
                default:
                    logHelper.Log("Uknown request received: " + request, LogType.Error);
                    break;
            }
            client.sb.Clear();
            if (client.connected)
                client.socket.BeginReceive(client.buffer, 0, Client.BufferSize, 0, new AsyncCallback(ReceiveCallback), client);
        }

        private static void Login(Client client, String password)
        {
            Boolean alreadyLoggedIn = false;
            foreach (Client connectedClient in clients)
            {

                if (connectedClient.loggedIn && connectedClient.name.Equals(client.name))
                {
                    alreadyLoggedIn = true;
                    logHelper.Log(client.name + " is already logged in refusing login.", LogType.Info);
                    clients.Remove(client);
                    client.Disconnect();
                    client.connected = false;
                    break;
                }
            }
            if (!alreadyLoggedIn)
            {
                if (mysqlHelper.CheckLoginCredentials(client, password))
                {
                    logHelper.Log(client.name + " succesfully logged in.", LogType.Info);
                    mysqlHelper.SetClientLoginStatus(client, true);
                }
                else
                {
                    logHelper.Log(client.name + " tried logging in with wrong credentials.", LogType.Info);
                    clients.Remove(client);
                    client.Disconnect();
                    client.connected = false;

                }
            }

        }

        private static void Send(Socket handler, String data)
        {
            // Convert the string data to byte data using ASCII encoding.  
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            // Begin sending the data to the remote device.  
            handler.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), handler);
        }

        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                Socket handler = (Socket)ar.AsyncState;
                int bytesSent = handler.EndSend(ar);
            }
            catch (Exception e)
            {
                logHelper.Log(e.ToString(), LogType.Error);
            }
        }

    }
}
