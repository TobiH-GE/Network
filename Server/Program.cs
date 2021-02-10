using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using ServerLogic;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Logic ServerLogic = new Logic();
            Console.WriteLine("server starting ...");
            ServerLogic.Start(); // TODO: async wait for start
            Console.WriteLine("listing on port xxx ...");
            WaitForData(ServerLogic);
            WaitForCommands(ServerLogic);
            Console.WriteLine("server closed connection!");
        }

        static async void WaitForData(Logic ServerLogic)
        {
            string message = "";
            Task<string> worker;

            ServerLogic.WaitForClient();
            Console.WriteLine("client connected, waiting for data ...");

            while (message != "stop" && ServerLogic.IsConnected)
            {
                worker = Task.Run(() => ServerLogic.WaitForData());
                message = await worker;
                Console.WriteLine("data received: " + message);
            }
            Console.WriteLine("stopping ...");
        }
        static void WaitForCommands(Logic ServerLogic)
        {
            string command = "";
            while (command != "/exit")
            {
                command = Console.ReadLine();
                if (command == "/stop") ServerLogic.Stop();
                if (command == "/status")
                {
                    if (ServerLogic.IsConnected) Console.WriteLine("status connected ...");
                    else Console.WriteLine("status disconnected ...");
                }
            }
            Console.WriteLine("closing application ...");
        }
    }
}
