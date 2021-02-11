using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerLogic
{
    class Client
    {
        public TcpClient connection;
        public string message;
        public int receivedBytes = 0;
        public byte[] data = new byte[1024];
        public Client(TcpClient connection)
        {
            this.connection = connection;
        }
        public bool IsConnected
        {
            get
            {
                if (connection != null && connection.Connected)
                    return true;
                else
                    return false;
            }
        }
    }

    class Logic
    {
        TcpListener listener;
        LinkedList<Client> connectedClients = new LinkedList<Client>(); //TODO: list with clients

        public async void Start_Async()
        {
            if (listener == null)
            {
                listener = new TcpListener(IPAddress.Any, 1337);
                listener.Start();
            }

            Console.WriteLine("waiting for clients ...");

            while (true) // TODO: unfinished code!
            {
                Console.WriteLine("client connected, waiting for data ...");
                Client newClient = new Client(await Task.Run(() => listener.AcceptTcpClient()));
                connectedClients.AddLast(newClient);
                _ = Task.Run(() => ReceiveData_Async(newClient)); 
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
