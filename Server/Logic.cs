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
        TcpClient connection;
        NetworkStream stream;

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

            Console.WriteLine("waiting for client ...");

            while (connection == null || connection.Connected == false)
            {
                connection = listener.AcceptTcpClient();
                worker = Task.Run(() => Task.Delay(1000));
                await worker;
            }
            Console.WriteLine("client connected, waiting for data ...");
            ReceiveData_Async();
        }
        async void ReceiveData_Async()
        {
            string message = "";
            int receivedBytes = 0;
            byte[] data = new byte[1024];

            while (IsConnected)
            {
                try
                {
                    receivedBytes = await connection.GetStream().ReadAsync(data.AsMemory(0, data.Length));
                    message = Encoding.ASCII.GetString(data, 0, receivedBytes);
                    Console.WriteLine("received: " + message);
                }
                catch
                {
                    Console.WriteLine("error: ...");
                    //TODO: error
                }
                if (receivedBytes < 1 || message[..4] == "stop")
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
            stream.Close();
            connection.Close();
            listener.Stop();
        }
    }
    
}
