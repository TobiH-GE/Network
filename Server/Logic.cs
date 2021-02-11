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
        public TcpClient _connection;
        public string message;
        public int receivedBytes = 0;
        public byte[] data = new byte[1024];
        public Client(TcpClient connection)
        {
            _connection = connection;
        }
    }

    class Logic
    {
        TcpListener listener;
        TcpClient connection;

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

        public async void Start_Async()
        {
            Task worker;
            if (listener == null)
            {
                listener = new TcpListener(IPAddress.Any, 1337);
                listener.Start();
            }

            Console.WriteLine("waiting for clients ...");

            while (true)
            {
                connection = listener.AcceptTcpClient();
                worker = Task.Run(() => Task.Delay(1000));
                await worker;
                Console.WriteLine("client connected, waiting for data ...");
                Client newClient = new Client(connection);
                Task.Run(() => ReceiveData_Async(newClient)); // TODO: unfinished code!
            }
        }
        async void ReceiveData_Async(Client client)
        {
            while (IsConnected)
            {
                try
                {
                    client.receivedBytes = await connection.GetStream().ReadAsync(client.data.AsMemory(0, client.data.Length));
                    client.message = Encoding.ASCII.GetString(client.data, 0, client.receivedBytes);
                    Console.WriteLine("received: " + client.message);
                }
                catch
                {
                    Console.WriteLine("error: ...");
                    //TODO: error
                }
                if (client.receivedBytes < 1 || client.message[..4] == "stop")
                {
                    Console.WriteLine("connection error, closing connection: ...");
                    connection.Close();
                }
            }
            Console.WriteLine("stopping ...");
            Start_Async();
        }
        public void Send(string message)
        {
            if (connection == null || !connection.Connected) return;
            byte[] data;
            data = Encoding.ASCII.GetBytes("server: " + message);
            connection.GetStream().Write(data, 0, data.Length);
            Console.WriteLine("sending: " + message);
        }
        public void Stop()
        {
            connection.Close();
            listener.Stop();
        }
    }
    
}
