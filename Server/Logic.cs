using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using NetworkMessage;

namespace ServerLogic
{
    class Logic
    {
        TcpListener listener;
        LinkedList<Client> connectedClients = new LinkedList<Client>();
        Action<string> consoleResponse;

        public Logic(Action<string> consoleResponse)
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
                consoleResponse.Invoke("client connected, waiting for data ...");
                Client newClient = new Client(connection);
                newClient.task = new Task(() => ReceiveData_Async(newClient));
                connectedClients.AddLast(newClient);
                newClient.task.Start();
            }
        }
        async void ReceiveData_Async(Client client)
        {
            while (client.IsConnected)
            {
                try
                {
                    client.receivedBytes = await client.connection.GetStream().ReadAsync(client.data.AsMemory(0, client.data.Length));
                    MessageType MessageType = (MessageType)client.data[0];
                    DataType DataType = (DataType)client.data[1];
                    byte ParamterLenght = client.data[2];
                    byte UsernameLenght = client.data[3];

                    if (DataType == DataType.Text)
                    {
                        int offset = 4;
                        string message = Encoding.ASCII.GetString(client.data, offset, client.receivedBytes - offset);
                        string parameter = message.Substring(0, ParamterLenght);
                        string username = message.Substring(ParamterLenght, UsernameLenght);
                        string text = message.Substring(ParamterLenght + UsernameLenght);
                        Send($"{username}: {text}"); //TODO: send message in correct format
                    }
                    else if (DataType == DataType.File) //TODO: file handling
                    {
                        int offset = 4;
                        string message = Encoding.ASCII.GetString(client.data, offset, ParamterLenght + UsernameLenght);
                        string parameter = message.Substring(0, ParamterLenght);
                        string username = message.Substring(ParamterLenght, UsernameLenght);
                        int fileoffset = offset + ParamterLenght + UsernameLenght;
                        byte[] file = client.data[fileoffset..];
                        Send($"{username}: (is sending a file)"); //TODO: send message in correct format
                    }
                }
                catch
                {
                    consoleResponse.Invoke("error: ...");
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
        public void Send(string message)
        {
            foreach (var client in connectedClients)
            {
                if (client.connection == null || !client.connection.Connected) return; //TODO: client.connection
                byte[] data;
                data = Encoding.ASCII.GetBytes(message);
                client.connection.GetStream().Write(data, 0, data.Length);
            }
            consoleResponse.Invoke("sending: " + message);
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
