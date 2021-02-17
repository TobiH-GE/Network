using System.ComponentModel;

namespace NetworkViewModel
{
    public partial class ViewModel
    {
        public class Room : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;

            string _name = "";
            int _users = 0;
            string _content;
            bool _selected = false;

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
            public bool Selected
            {
                get { return _selected; }
                set
                {
                    if (_selected != value)
                    {
                        _selected = value;
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Selected)));
                    }
                }
            }
        }
    }
}
