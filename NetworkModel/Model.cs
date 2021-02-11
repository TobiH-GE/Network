using System;
using System.Net.Sockets;
using System.Text;

namespace NetworkModel
{
    public static class TCPConnection
    {
        static TcpClient connection = new TcpClient();
        static NetworkStream dataStream;
        public static Action<string> OnReceive;
        public static void Connect(string address, int port)
        {
            connection.Connect(address, port);
            dataStream = connection.GetStream();

            ReceiveData_Async();
        }
        public static async void ReceiveData_Async()
        {
            string message = "";

            byte[] data = new byte[1024];
            int receivedBytes;
            while (connection.Connected)
            {
                receivedBytes = await connection.GetStream().ReadAsync(data.AsMemory(0, data.Length));
                message = Encoding.ASCII.GetString(data, 0, receivedBytes);
                OnReceive.Invoke(message);
            }
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
