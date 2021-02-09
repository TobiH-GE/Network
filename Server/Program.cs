using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("server running ...");
            TcpListener listener = new TcpListener(IPAddress.Any, 1337);
            Console.WriteLine("listing on port xxx ...");
            listener.Start();

            TcpClient connection = listener.AcceptTcpClient();
            Console.WriteLine("client connected");

            var stream = connection.GetStream();

            int recievedBytes;
            byte[] data = new byte[1024];

            while ((recievedBytes = stream.Read(data, 0, data.Length)) != 0)
            {
                string message = Encoding.ASCII.GetString(data, 0, recievedBytes);

                Console.WriteLine("client msg: {0}", message);
            }

            stream.Close();
            connection.Close();
            listener.Stop();

            Console.WriteLine("server closed connection!");
        }
    }
}
