using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using NetworkModel;

namespace NetworkViewModel
{
    public class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand Connect { get; init; }
        public ICommand Send { get; init; }
        public ICommand Disconnect { get; init; }
        public ICommand Join { get; init; }

        Random rnd = new Random();

        string _username = "";
        int _roomID = 1; // TODO: remove!
        string _password = "password";
        string _address = "127.0.0.1";
        int _port = 1337;
        string _message = "";
        bool _isConnected = false;
        string _statusContent = "";

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

            TCPConnection.OnReceive = Receive;
            TCPConnection.OnJoinOk = JoinRoom;
            TCPConnection.OnLeaveOk = LeaveRoom;

            Username = rnd.Next(10000, 99999).ToString(); // random name

            Rooms.Add(new Room() { Name = "Status", Users = 2 });
        }
        public class Room : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;

            string _name = "";
            int _users = 0;
            string _content;
            int _height = 0;

            public string Name
            {
                get { return _name; }
                set
                {
                    if (_name != value)
                    {
                        _name = value;
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name)));
                    }
                }
            }
            public int Users
            {
                get { return _users; }
                set
                {
                    if (_users != value)
                    {
                        _users = value;
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Users)));
                    }
                }
            }
            public string Content
            {
                get { return _content; }
                set
                {
                    if (_content != value)
                    {
                        _content = value;
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Content)));
                    }
                }
            }
            public int Height
            {
                get { return _height; }
                set
                {
                    if (_height != value)
                    {
                        _height = value;
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Height)));
                    }
                }
            }
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
        public int RoomID
        {
            get { return _roomID; }
            set
            {
                if (_roomID != value)
                {
                    _roomID = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RoomID)));
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
        public string StatusContent
        {
            get { return _statusContent; }
            set
            {
                if (_statusContent != value)
                {
                    _statusContent = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(StatusContent)));
                }
            }
        }
        void Receive(string destination, string message)
        {
            if (destination == "Status")
                StatusContent += message + "\n";
            else
            {
                foreach (var item in Rooms)
                {
                    if (item.Name == destination)
                    {
                        item.Height = 400;
                        item.Content += message + "\n";
                    }
                    else
                    {
                        item.Height = 0;
                    }
                }
            }
        }
        void JoinRoom(string room)
        {
            Rooms.Add(new Room() { Name = room, Users = 2 });
        }
        void LeaveRoom(string room)
        {
            foreach (var item in Rooms)
            {
                if (item.Name == room)
                {
                    Rooms.Remove(item);
                    break;
                }
            }
        }
    }
}
