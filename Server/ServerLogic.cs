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
                    client.receivedBytes = await client.connection.GetStream().ReadAsync(client.data.AsMemory(0, client.data.Length));
                    MsgType MessageType = (MsgType)client.data[0];
                    SubType SubType = (SubType)client.data[1];
                    if (MessageType == MsgType.Command)
                    {
                        consoleResponse.Invoke("incoming command ...");
                        MessageCommand incomingMessage = new MessageCommand(client.data);
                        if (SubType == SubType.Login)
                        {
                            if (incomingMessage.Parameter == "password")
                                consoleResponse.Invoke("password correct ...");
                            else
                                consoleResponse.Invoke("password wrong ...");

                            // do something;
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
                        consoleResponse.Invoke("incoming text message ...");
                        SendAll(incomingMessage);
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
            foreach (var client in connectedClients)
            {
                if (client.connection == null || !client.connection.Connected) return; //TODO: client.connection
                byte[] data;
                data = Encoding.ASCII.GetBytes(message);
                client.connection.GetStream().Write(data, 0, data.Length);
            }
            consoleResponse.Invoke("sending text to all: " + message);
        }
        public void SendAll(Message message)
        {
            foreach (var client in connectedClients)
            {
                if (client.connection == null || !client.connection.Connected) return; //TODO: client.connection
                byte[] data;
                data = message.getBytes();
                client.connection.GetStream().Write(data, 0, data.Length);
            }
            consoleResponse.Invoke("sending message to all ... ");
        }
        public void Send(Client client, Message message)
        {
            if (client.connection == null || !client.connection.Connected) return; //TODO: client.connection
            {
                byte[] data;
                data = message.getBytes();
                client.connection.GetStream().Write(data, 0, data.Length);
                consoleResponse.Invoke("sending message to client ... ");
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
