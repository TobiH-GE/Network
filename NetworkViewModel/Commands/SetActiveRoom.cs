using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using NetworkModel;
using NetworkMessage;

namespace NetworkViewModel
{
    class SetActiveRoom : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public ViewModel Parent;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            Parent.SelectedRoom = (string)parameter;
        }
    }
}