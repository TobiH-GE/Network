using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerLogic
{
    class Logic
    {
        TcpListener listener;
        LinkedList<Client> connectedClients = new LinkedList<Client>(); //TODO: list with clients
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

            while (true) // TODO: unfinished code!
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
                    client.message = Encoding.ASCII.GetString(client.data, 0, client.receivedBytes);
                    consoleResponse.Invoke("received: " + client.message);
                    Send(client.message);
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
                data = Encoding.ASCII.GetBytes("server: " + message);
                client.connection.GetStream().Write(data, 0, data.Length);
            }
            consoleResponse.Invoke("sending: " + message);
        }
        public void Status()
        {
            string returnString = "";
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
