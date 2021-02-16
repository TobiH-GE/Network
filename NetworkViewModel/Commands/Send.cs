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
            if (((string)parameter).Length >= 7 && ((string)parameter)[..5] == "/join")
            {
                Message outgoingMessage = new MessageCommand(MsgType.Command, SubType.JoinRoom, ((string)parameter)[6..], Parent.Username, "");
                TCPConnection.Send(outgoingMessage);
            }
            else if (((string)parameter).Length >= 7 && ((string)parameter)[..5] == "/send")
            {
                string[] array = ((string)parameter).Split(' ');
                Message outgoingMessage = new MessageText(MsgType.Text, SubType.Direct, array[1], Parent.Username, array[2]);
                TCPConnection.Send(outgoingMessage);
            }
            else if (((string)parameter).Length >= 7 && ((string)parameter)[..5] == "/room")
            {
                string[] array = ((string)parameter).Split(' ');
                if (array[0] == null || array[1] == null || array[2] == null) return;
                Message outgoingMessage = new MessageText(MsgType.Text, SubType.Room, array[1], Parent.Username, array[2]);
                TCPConnection.Send(outgoingMessage);
            }
            else if (((string)parameter).Length >= 8 && ((string)parameter)[..6] == "/leave")
            {
                string[] array = ((string)parameter).Split(' ');
                Message outgoingMessage = new MessageCommand(MsgType.Command, SubType.LeaveRoom, ((string)parameter)[7..], Parent.Username, "");
                TCPConnection.Send(outgoingMessage);
            }
            else
            {
                Message outgoingMessage = new MessageText(MsgType.Text, SubType.Room, Parent.SelectedRoom, Parent.Username, (string)parameter);
                TCPConnection.Send(outgoingMessage);
            }
        }
    }
}