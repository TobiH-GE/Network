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
        public void Start()
        {
            if (listener == null)
            {
                listener = new TcpListener(IPAddress.Any, 1337);
                listener.Start();
            }

            Console.WriteLine("waiting for clients ...");

            while (true) // TODO: unfinished code!
            {
                TcpClient connection = listener.AcceptTcpClient();
                Console.WriteLine("client connected, waiting for data ...");
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
                    Console.WriteLine("received: " + client.message);
                    Send(client.message);
                }
                catch
                {
                    Console.WriteLine("error: ...");
                    //TODO: error
                }
                if (client.receivedBytes < 1 || client.message[..4] == "stop")
                {
                    Console.WriteLine("connection error, closing connection: ...");
                    client.connection.Close();
                }
            }
            Console.WriteLine("stopping ...");
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
                Console.WriteLine("sending: " + message);
            }
        }
        public void Status()
        {
            foreach (var client in connectedClients)
            {
                Console.WriteLine($"client: xx, connected: { client.connection.Connected }, IP: { client.connection.Client.RemoteEndPoint}");
            }
        }
        public void Stop()
        {
            foreach (var client in connectedClients)
            {
                client.connection.Close(); //TODO: client.connection
            }
            listener.Stop();
        }
    }
}
