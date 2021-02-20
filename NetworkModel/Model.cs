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

        public static Action<string,string> OnReceive;
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
            byte[] receivedData;
            byte[] receivedDataDebug; // debug
            int receivedBytes = 0;
            while (connection.Connected)
            {
                receivedData = new byte[1024];
                receivedDataDebug = new byte[1024]; // debug

                receivedBytes = await connection.GetStream().ReadAsync(receivedData.AsMemory(0, receivedData.Length), cts.Token);
                receivedDataDebug = receivedData;
                int MessageSize = BitConverter.ToInt32(receivedData, 4);

                //OnReceive.Invoke("Status", $"received data with receivedBytes {receivedBytes} MessageSize {MessageSize} ...");
                do
                {
                    byte[] data = receivedData[..MessageSize];
                    MsgType MessageType = (MsgType)data[0];
                    SubType SubType = (SubType)data[1];

                    if (MessageType == MsgType.Command)
                    {
                        MessageCommand incomingMessage = new MessageCommand(data);
                        if (SubType == SubType.LoginRequest)
                        {
                            OnReceive.Invoke("Status", "server is requesting login data ...");
                            Send(new MessageCommand(MsgType.Command, SubType.Login, "password", Username));
                            // do something
                        }
                        else if (SubType == SubType.JoinOk)
                        {
                            OnReceive.Invoke("Status", $"joining room {incomingMessage.Parameter}.");
                            OnJoinOk.Invoke(incomingMessage.Parameter);
                        }
                        else if (SubType == SubType.LeaveOk)
                        {
                            OnReceive.Invoke("Status", $"leaving room {incomingMessage.Parameter}.");
                            OnLeaveOk.Invoke(incomingMessage.Parameter);
                        }
                        else if (SubType == SubType.Userlist)
                        {
                            OnReceive.Invoke("Status", $"received userlist room: {incomingMessage.Parameter}.");
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

                        if (SubType == SubType.Room)
                        {
                            OnReceive.Invoke(incomingMessage.Parameter, incomingMessage.Username + ": " + incomingMessage.Text);
                        }
                        else
                        {
                            OnReceive.Invoke("Status", incomingMessage.Username + ": " + incomingMessage.Text);
                        }
                    }
                    if (receivedBytes > MessageSize + 8) // additional message with minimum message size in buffer, check if valid
                    {
                        receivedData = receivedData[MessageSize..];
                        MessageSize = BitConverter.ToInt32(receivedData, 4);
                    }
                    else
                    {
                        MessageSize = 0;
                    }
                } while (MessageSize > 0);
                
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
