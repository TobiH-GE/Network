using System.Net.Sockets;
using System.Threading.Tasks;

namespace Server
{
    class Client
    {
        public TcpClient connection;
        public Task task;
        public int receivedBytes = 0;
        public byte[] data = new byte[1024];
        public Client(TcpClient connection)
        {
            this.connection = connection;
        }
        public bool IsConnected
        {
            get
            {
                if (connection != null && connection.Connected)
                    return true;
                else
                    return false;
            }
        }
    }
}
