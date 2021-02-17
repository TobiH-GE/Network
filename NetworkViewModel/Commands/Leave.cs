using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using NetworkMessage;
using NetworkModel;

namespace NetworkViewModel
{
    class Leave : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public ViewModel Parent;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            Message outgoingMessage = new MessageCommand(MsgType.Command, SubType.LeaveRoom, ((string)parameter), Parent.Username, "");
            TCPConnection.Send(outgoingMessage);
        }
    }
}
