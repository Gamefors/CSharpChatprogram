namespace ClientGUI.Dir.ViewModel
{
    class MainWindowViewModel : BaseViewModel
    {
        private LoginViewModel LoginViewModel { get; set; }

        public BaseViewModel ActiveViewModel { get; set; }

        public MainWindowViewModel()
        {
            LoginViewModel = new LoginViewModel();

            ActiveViewModel = LoginViewModel;

        }

    }
}
