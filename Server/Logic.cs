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
            get { return connection.Connected; }
        }

        public void Start()
        {
            listener = new TcpListener(IPAddress.Any, 1337);
            listener.Start();
        }
        public bool WaitForClient()
        {
            connection = listener.AcceptTcpClient();
            while (connection.Connected == false)
            {
                Task.Delay(100);
            }
            stream = connection.GetStream();
            return true;
        }
        public string WaitForData()
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
