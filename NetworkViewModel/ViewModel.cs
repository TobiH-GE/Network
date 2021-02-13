using System;
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

        Random rnd = new Random();

        string _username = "";
        int _group = 1;
        string _password = "password";
        string _address = "127.0.0.1";
        int _port = 1337;
        string _message = "";
        bool _isConnected = false;
        string _chatBox = "";

        public ViewModel()
        {
            Connect = new Connect() { Parent = this };
            Send = new Send() { Parent = this };
            Disconnect = new Disconnect() { Parent = this };

            TCPConnection.OnReceive = Receive;

            Username = rnd.Next(10000, 99999).ToString(); // random name
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
        public int Group
        {
            get { return _group; }
            set
            {
                if (_group != value)
                {
                    _group = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Group)));
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
        public string ChatBox
        {
            get { return _chatBox; }
            set
            {
                if (_chatBox != value)
                {
                    _chatBox = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ChatBox)));
                }
            }
        }
        void Receive(string message)
        {
            ChatBox += message + "\n";
        }
    }
}
