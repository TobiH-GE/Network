using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Server
{
    class Client
    {
        public TcpClient connection;
        public Task task;
        public string Username;
        public int receivedBytes = 0;
        public byte[] data = new byte[1024];
        public LinkedList<string> Rooms = new LinkedList<string>();
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
        public bool Send(byte[] data)
        {
            if (connection == null || !connection.Connected) return false;
            connection.GetStream().Write(data, 0, data.Length);
            return true;
        }
    }
}
