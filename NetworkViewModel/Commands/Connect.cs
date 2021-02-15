using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using NetworkModel;

namespace NetworkViewModel
{
    class Connect : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public ViewModel Parent;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            TCPConnection.Username = Parent.Username;
            TCPConnection.Connect(Parent.Address, Parent.Port);
            Parent.IsConnected = true;
        }
    }
}
