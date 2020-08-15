using System;
using System.Windows.Input;

namespace ClientGUI
{
    public class RelayCommand : ICommand
    {
        private Action mAction;
        private Action<object> loginButtonAction;

        public event EventHandler CanExecuteChanged = (sender, e) => { };

        public RelayCommand(Action action)
        {
            mAction = action;
        }

        public RelayCommand(Action<object> loginButtonAction)
        {
            this.loginButtonAction = loginButtonAction;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            if(mAction == null)
            {
                loginButtonAction(parameter);
            }
            else
            {
                mAction();
            }
            
        }


    }
}