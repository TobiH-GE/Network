using NetworkViewModel;
using System.Windows;
using System.Windows.Controls;

namespace Network
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new ViewModel();
        }
        private void File_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // Note that you can have more than one file.
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                var file = files[0];
                // HandleFile(file);
            }
        }
        private void RoomButton_Click(object sender, RoutedEventArgs e)
        {
            ((ViewModel)DataContext).SelectedRoom = ((Button)sender).Content.ToString();
        }
    }
}
