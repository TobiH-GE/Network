using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using NetworkMessage;

namespace NetworkModel
{
    public static class TCPConnection
    {
        static TcpClient connection;
        static NetworkStream dataStream;
        static CancellationTokenSource cts;

        public static Action<string> OnReceive;
        public static void Connect(string address, int port)
        {
            connection = new TcpClient();
            connection.Connect(address, port);
            dataStream = connection.GetStream();
            cts = new CancellationTokenSource();

            ReceiveData_Async();
        }
        public static async void ReceiveData_Async()
        {
            Message message;

            byte[] data = new byte[1024];
            int receivedBytes = 0;
            while (connection.Connected)
            {
                try
                {
                    receivedBytes = await connection.GetStream().ReadAsync(data.AsMemory(0, data.Length), cts.Token);
                    message = new Message(data);
                    OnReceive.Invoke(message.Text);
                }
                catch (Exception)
                {
                    OnReceive.Invoke("(connection error or connection aborted)");
                    Disconnect();
                }
                if (receivedBytes < 1) // TODO: optimize, remove bug
                {
                    Console.WriteLine("connection error, closing connection: ...");
                    connection.Close();
                }
            }
        }
        public static void Send(MessageType messagetype, DataType datatype, string parameter, string username, string message)
        {
            string datastring = $"    {parameter}{username}{message}";
            byte[] data = Encoding.ASCII.GetBytes(datastring);
            data[0] = (byte)messagetype;
            data[1] = (byte)datatype;
            data[2] = (byte)parameter.Length;
            data[3] = (byte)username.Length;
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
