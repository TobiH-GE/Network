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

            listener = new TcpListener(IPAddress.Any, 1337);
            listener.Start();

            Console.WriteLine("waiting for client ...");

            while (connection == null || connection.Connected == false)
            {
                connection = listener.AcceptTcpClient();
                worker = Task.Run(() => Task.Delay(1000));
                await worker;
            }
            Console.WriteLine("client connected, waiting for data ...");
            stream = connection.GetStream();
            ReceiveData_Async();
        }
        async void ReceiveData_Async()
        {
            string message = "";
            Task<string> worker;

            while (message != "stop" && IsConnected)
            {
                worker = Task.Run(() => ReceiveData());
                message = await worker;
                Console.WriteLine("data received: " + message);
            }
            Console.WriteLine("stopping ...");
        }
        public void Send(string message)
        {
            if (connection == null || !connection.Connected) return;
            byte[] data;
            data = Encoding.ASCII.GetBytes("server: " + message);
            connection.GetStream().Write(data, 0, data.Length);
            Console.WriteLine("sending: " + message);
        }
        string ReceiveData()
        {
            int receivedBytes;
            byte[] data = new byte[1024];
            string message = "";

            try
            {
                while ((receivedBytes = stream.Read(data, 0, data.Length)) == 0)
                {
                    Task.Delay(100);
                }

                message = Encoding.ASCII.GetString(data, 0, receivedBytes);
            }
            catch
            {
                //TODO: error
            }
            
            return message;
        }
        public void Stop()
        {
            stream.Close();
            connection.Close();
            listener.Stop();
        }
    }
    
}
