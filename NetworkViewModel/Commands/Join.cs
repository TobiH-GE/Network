using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using NetworkMessage;
using NetworkModel;

namespace NetworkViewModel
{
    class Join : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public ViewModel Parent;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            Message outgoingMessage = new MessageCommand(MsgType.Command, SubType.JoinRoom, ((string)parameter), Parent.Username, "");
            TCPConnection.Send(outgoingMessage);
            outgoingMessage = new MessageCommand(MsgType.Command, SubType.Userlist, ((string)parameter), Parent.Username, "");
            TCPConnection.Send(outgoingMessage);
        }
    }
}
