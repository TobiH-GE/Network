﻿using System;
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
            if (Parent.TestUser == "")
            {
                Message outgoingMessage = new MessageText(MsgType.Text, SubType.Group, Parent.Group.ToString(), Parent.Username, (string)parameter);
                TCPConnection.Send(outgoingMessage);
            }
            else //TODO: remove? just for testing
            {
                Message outgoingMessage = new MessageText(MsgType.Text, SubType.Direct, Parent.TestUser, Parent.Username, (string)parameter);
                TCPConnection.Send(outgoingMessage);
            }
        }
    }
}