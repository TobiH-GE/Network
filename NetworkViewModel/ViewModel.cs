using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using NetworkModel;

namespace NetworkViewModel
{
    public partial class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand Connect { get; init; }
        public ICommand Send { get; init; }
        public ICommand Disconnect { get; init; }
        public ICommand Join { get; init; }
        public ICommand Leave { get; init; }
        public ICommand SetActiveRoom { get; init; }

        Random rnd = new Random();

        string _username = "";
        string _password = "password";
        string _address = "127.0.0.1";
        int _port = 1337;
        string _message = "";
        bool _isConnected = false;
        string _selectedRoom = "";

        ObservableCollection<Room> _rooms = new ObservableCollection<Room>();
        public ObservableCollection<Room> Rooms
        {
            get { return _rooms; }
            set
            {
                if (_rooms != value)
                {
                    _rooms = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Rooms)));
                }
            }
        }

        public ViewModel()
        {
            Connect = new Connect() { Parent = this };
            Send = new Send() { Parent = this };
            Disconnect = new Disconnect() { Parent = this };
            Join = new Join() { Parent = this };
            Leave = new Leave() { Parent = this };
            SetActiveRoom = new SetActiveRoom() { Parent = this };

            TCPConnection.OnReceive = Receive;
            TCPConnection.OnJoinOk = JoinRoom;
            TCPConnection.OnLeaveOk = LeaveRoom;

            Username = rnd.Next(10000, 99999).ToString(); // random name

            Rooms.Add(new Room() { Name = "Status", Users = 0, Selected = true });
        }
        public string Username
        {
            get { return _username; }
            set
            {
                if (_username != value)
                {
                    _username = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Username)));
                }
            }
        }
        public string SelectedRoom
        {
            get { return _selectedRoom; }
            set
            {
                if (_selectedRoom != value)
                {
                    _selectedRoom = value;
                    foreach (var item in Rooms)
                    {
                        if (item.Name == value)
                        {
                            item.Selected = true;
                        }
                        else
                        {
                            item.Selected = false;
                        }
                    }
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedRoom)));
                }
            }
        }
        public string Password
        {
            get { return _password; }
            set
            {
                if (_password != value)
                {
                    _password = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Password)));
                }
            }
        }
        public string Address
        {
            get { return _address; }
            set
            {
                if (_address != value)
                {
                    _address = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Address)));
                }
            }
        }
        public int Port
        {
            get { return _port; }
            set
            {
                if (_port != value)
                {
                    _port = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Port)));
                }
            }
        }
        public string Message
        {
            get { return _message; }
            set
            {
                if (_message != value)
                {
                    _message = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Message)));
                }
            }
        }
        public bool IsConnected
        {
            get { return _isConnected; }
            set
            {
                if (_isConnected != value)
                {
                    _isConnected = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsConnected)));
                }
            }
        }
        void Receive(string destination, string message) //TODO: move to model
        {
            foreach (var item in Rooms)
            {
                if (item.Name == destination)
                {
                    item.Content += message + "\n";
                }
            }
        }
        void JoinRoom(string room) //TODO: move to model
        {
            Rooms.Add(new Room() { Name = room, Users = 2 });
            SelectedRoom = room;
        }
        void LeaveRoom(string room) //TODO: move to model
        {
            foreach (var item in Rooms)
            {
                if (item.Name == room)
                {
                    Rooms.Remove(item);
                    SelectedRoom = "Status";
                    break;
                }
            }
        }
    }
}
