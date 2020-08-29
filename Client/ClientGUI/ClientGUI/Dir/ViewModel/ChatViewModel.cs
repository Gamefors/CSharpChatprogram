using ClientGUI.Dir.Objects;
using ClientGUI.Dir.Utils.CSocket;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace ClientGUI.Dir.ViewModel
{
    class ChatViewModel : BaseViewModel
    {
       
        private ServerHandler serverHandler;
        public ObservableCollection<ClientObject> Clients { 
            get
            {
                return serverHandler.clients;
            }
            set { Clients = value; }
        }
        public string UserListOnlineLabel { get; set; }

        private Config config;
        public Action<string> EnterPressEvent;
        public string Input { get; set; }
        public string Output { get; set; }
        public ICommand EnterKeyCommand { get; set; }
        public ChatViewModel(Action<string> EnterPressEvent_, Config config_, ServerHandler serverHandler_)
        {
            serverHandler = serverHandler_;
            UserListOnlineLabel = "Online-" + Clients.Count;
            config = config_;
            EnterPressEvent = EnterPressEvent_;
            EnterKeyCommand = new RelayCommand(EnterKeyAction);
        }
        

        public void EnterKeyAction()
        {
            string input = "[" + config.loginName + "]: " + Input ?? "";
            Output = Output + "\n" + input;
            EnterPressEvent(Input ?? "");
            Input = "";

            //TODO: make this update when new user is added or removed from list
            UserListOnlineLabel = "Online-" + Clients.Count;
        }

        public void writeOutput(string msg)
        {
            string input = msg ?? "";
            Output = Output + "\n" + input;
        }
    }
}
