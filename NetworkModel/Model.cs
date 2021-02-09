using System;
using System.Net.Sockets;
using System.Text;

namespace NetworkModel
{
    public static class TCPConnection
    {
        static TcpClient connection = new TcpClient();
        static NetworkStream dataStream;
        public static void Connect(string address, int port)
        {
            connection.Connect(address, port);
            dataStream = connection.GetStream();
        }
        public static void Send(string message)
        {
            byte[] data = Encoding.ASCII.GetBytes(message);
            dataStream.Write(data, 0, data.Length);
        }
        public static void Disconnect()
        {
            dataStream.Flush();
            dataStream.Close();
            connection.Close();
        }
    }
}
