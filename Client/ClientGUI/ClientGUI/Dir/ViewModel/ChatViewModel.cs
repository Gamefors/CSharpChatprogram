using System;
using System.Windows.Input;

namespace ClientGUI.Dir.ViewModel
{
    class ChatViewModel : BaseViewModel
    {
        public Action<string> EnterPressEvent;
        public string Input { get; set; }
        public string Output { get; set; }
        public ICommand EnterKeyCommand { get; set; }
        public ChatViewModel(Action<string> EnterPressEvent_)
        {
            EnterPressEvent = EnterPressEvent_;
            EnterKeyCommand = new RelayCommand(EnterKeyAction);
        }

        public void EnterKeyAction()
        {
            string input = Input ?? "";
            Output = Output + "\n" + input;
            EnterPressEvent(input);
            Input = "";
        }

        public void writeOutput(string msg)
        {
            string input = msg ?? "";
            Output = Output + "\n" + input;
        }
    }
}
