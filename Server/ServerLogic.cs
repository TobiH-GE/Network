using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using NetworkMessage;

namespace Server
{
    class ServerLogic
    {
        TcpListener listener;
        LinkedList<Client> connectedClients = new LinkedList<Client>();
        Action<string> consoleResponse;

        public ServerLogic(Action<string> consoleResponse)
        {
            this.consoleResponse = consoleResponse;
        }
        public void Start()
        {
            if (listener == null)
            {
                listener = new TcpListener(IPAddress.Any, 1337);
                listener.Start();
            }
            consoleResponse.Invoke("waiting for clients ...");

            while (true)
            {
                TcpClient connection = listener.AcceptTcpClient();
                consoleResponse.Invoke("client connected ...");
                Client newClient = new Client(connection);
                newClient.task = new Task(() => ReceiveData_Async(newClient));
                connectedClients.AddLast(newClient);
                newClient.task.Start();
                consoleResponse.Invoke("sending login request to client ...");
                Send(newClient, new MessageCommand(MsgType.Command, SubType.LoginRequest));
            }
        }
        async void ReceiveData_Async(Client client)
        {
            while (client.IsConnected)
            {
                try
                {
                    client.data = new byte[1024];
                    client.receivedBytes = await client.connection.GetStream().ReadAsync(client.data.AsMemory(0, client.data.Length)); // TODO: use client.data.AsMemory???
                    MsgType MessageType = (MsgType)client.data[0];
                    SubType SubType = (SubType)client.data[1];

                    if (MessageType == MsgType.Command)
                    {
                        consoleResponse.Invoke("incoming command ...");
                        MessageCommand incomingMessage = new MessageCommand(client.data);
                        if (SubType == SubType.Login) //TODO: check correct password in database
                        {
                            if (incomingMessage.Parameter == "password") 
                            {
                                consoleResponse.Invoke($"user {incomingMessage.Username} password correct ...");
                                client.Username = incomingMessage.Username;
                                Send(client, new MessageText(MsgType.Text, SubType.Info, "", "server", "login successful!"));
                            }
                            else
                            {
                                consoleResponse.Invoke($"user {incomingMessage.Username} password wrong ...");
                                Send(client, new MessageText(MsgType.Text, SubType.Info, "", "server", "password wrong!"));
                            }
                        }
                        else if(SubType == SubType.JoinRoom)
                        {
                            client.Rooms.AddLast(incomingMessage.Parameter);
                            Send(client, new MessageCommand(MsgType.Command, SubType.JoinOk, incomingMessage.Parameter, "", ""));
                            Send(client, new MessageText(MsgType.Text, SubType.Room, incomingMessage.Parameter, "server", $"you joined room {incomingMessage.Parameter}."));
                        }
                        else if (SubType == SubType.LeaveRoom)
                        {
                            client.Rooms.Remove(incomingMessage.Parameter);
                            Send(client, new MessageCommand(MsgType.Command, SubType.LeaveOk, incomingMessage.Parameter, "", ""));
                            Send(client, new MessageText(MsgType.Text, SubType.Info, "", "server", $"you left room {incomingMessage.Parameter}."));
                        }
                    }
                    else if (MessageType == MsgType.Data)
                    {
                        MessageData incomingMessage = new MessageData(client.data);
                        consoleResponse.Invoke("incoming data message ...");
                        SendAll(incomingMessage);
                    }
                    else if (MessageType == MsgType.Text)
                    {
                        MessageText incomingMessage = new MessageText(client.data);
                        if (SubType == SubType.Direct)
                        {
                            consoleResponse.Invoke("incoming direct text message ...");
                            Send(incomingMessage);
                        }
                        else if (SubType == SubType.Room)
                        {
                            consoleResponse.Invoke("incoming group text message ...");
                            SendGroup(incomingMessage); //TODO: send only to room
                        }
                        else if (SubType == SubType.Broadcast)
                        {
                            consoleResponse.Invoke("incoming broadcast text message ...");
                            SendAll(incomingMessage);
                        }
                    }
                    else
                    {
                        consoleResponse.Invoke("incoming unknown message type ...");
                    }
                }
                catch
                {
                    consoleResponse.Invoke("ReceiveData_Async error: ...");
                    //TODO: error
                }
                if (client.receivedBytes < 1)
                {
                    consoleResponse.Invoke("connection error, closing connection: ...");
                    client.connection.Close();
                }
            }
            consoleResponse.Invoke("stopping ...");
            connectedClients.Remove(client);
        }
        public void SendAll(string message)
        {
            byte[] data = Encoding.ASCII.GetBytes(message);
            foreach (var client in connectedClients)
            {
                if (!client.Send(data)) return;
            }
            consoleResponse.Invoke("sending text to all: " + message);
        }
        public void SendAll(Message message)
        {
            byte[] data = message.getBytes();
            foreach (var client in connectedClients)
            {
                if (!client.Send(data)) return;
            }
            consoleResponse.Invoke("sending message to all ... ");
        }
        public void SendGroup(Message message)
        {
            foreach (var client in connectedClients)
            {
                if (client.connection == null || !client.connection.Connected) return;
                if (client.Rooms.Contains(message.Parameter))
                {
                    client.Send(message.getBytes());
                }
            }
            consoleResponse.Invoke("sending message to group ... ");
        }
        public void Send(Client client, Message message)
        {
            if (!client.Send(message.getBytes())) return;
            consoleResponse.Invoke("sending message to client ... ");
        }
        public void Send(Message message)
        {
            foreach (var client in connectedClients)
            {
                if (client.connection == null || !client.connection.Connected) return;
                if (client.Username == message.Parameter)
                {
                    client.Send(message.getBytes());
                    consoleResponse.Invoke("sending message to username ... ");
                    break;
                }
            }
        }
        public void Status()
        {
            string returnString = "connected clients:\n";
            foreach (var client in connectedClients)
            {
                returnString+=$"client: xx, connected: { client.connection.Connected }, IP: { client.connection.Client.RemoteEndPoint}";
            }
            consoleResponse.Invoke(returnString);
        }
        public void Stop()
        {
            foreach (var client in connectedClients)
            {
                client.connection.Close();
            }
            listener.Stop();

            consoleResponse.Invoke("server stopped ...");
        }
    }
}
