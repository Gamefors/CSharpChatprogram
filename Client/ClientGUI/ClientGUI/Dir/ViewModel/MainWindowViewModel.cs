using System;

namespace ClientGUI.Dir.ViewModel
{
    class MainWindowViewModel : BaseViewModel
    {
        public event Action<bool> ConnectEvent;
        public event Action<string> loginEvent;

        public event Action<string> EnterPressEvent;

        private LoginViewModel LoginViewModel { get; set; }
        public BaseViewModel ActiveViewModel { get; set; }

        public MainWindowViewModel()
        {
            
            LoginViewModel = new LoginViewModel(this, ConnectEvent, loginEvent, EnterPressEvent);
            

            setActiveViewMode(LoginViewModel);
        }

        public void setActiveViewMode(BaseViewModel viewModel)
        {
            ActiveViewModel = viewModel;
        }

    }
}
