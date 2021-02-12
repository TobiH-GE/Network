using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using NetworkModel;
using NetworkMessage;

namespace NetworkViewModel
{
    class Send : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public ViewModel Parent;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            TCPConnection.Send((MessageType)4, (DataType)0 , Parent.Group.ToString(), Parent.Name, Parent.Message);
        }
    }
}