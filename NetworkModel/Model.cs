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
        public static string Username = "";

        public static Action<string> OnReceive;
        public static Action<string> OnJoinOk;
        public static Action<string> OnLeaveOk;
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
            byte[] data = new byte[1024];
            int receivedBytes = 0;
            while (connection.Connected)
            {
                try
                {
                    receivedBytes = await connection.GetStream().ReadAsync(data.AsMemory(0, data.Length), cts.Token);
                    MsgType MessageType = (MsgType)data[0];
                    SubType SubType = (SubType)data[1];

                    if (MessageType == MsgType.Command)
                    {
                        MessageCommand incomingMessage = new MessageCommand(data);
                        if (SubType == SubType.LoginRequest)
                        {
                            OnReceive.Invoke("server is requesting login data ...");
                            Send(new MessageCommand(MsgType.Command, SubType.Login, "password", Username));
                            // do something
                        }
                        else if(SubType == SubType.JoinOk)
                        {
                            OnReceive.Invoke($"joining room {incomingMessage.Parameter}.");
                            OnJoinOk.Invoke(incomingMessage.Parameter);
                        }
                        else if (SubType == SubType.LeaveOk)
                        {
                            OnReceive.Invoke($"leaving room {incomingMessage.Parameter}.");
                            OnLeaveOk.Invoke(incomingMessage.Parameter);
                        }
                    }
                    else if (MessageType == MsgType.Data)
                    {
                        MessageData incomingMessage = new MessageData(data);
                        // do something
                    }
                    else if (MessageType == MsgType.Text)
                    {
                        MessageText incomingMessage = new MessageText(data);
                        OnReceive.Invoke(incomingMessage.Username + ": " + incomingMessage.Text);
                    }
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
        public static void Send(Message message)
        {
            byte[] data = message.getBytes();
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
