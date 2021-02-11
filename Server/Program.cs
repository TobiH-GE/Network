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
            Start(ServerLogic);
            WaitForCommands(ServerLogic);
        }
        static async void Start(Logic ServerLogic)
        {
            await Task.Run(() => ServerLogic.Start());
        }
        static void WaitForCommands(Logic ServerLogic)
        {
            string command = "";
            while (command != "/exit")
            {
                command = Console.ReadLine();
                if (command == "/stop")
                {
                    ServerLogic.Stop();
                    Console.WriteLine("server closed connection!");
                }
                if (command == "/status")
                {
                    ServerLogic.Status();
                }
                if (command[..5] == "/send")
                {
                    ServerLogic.Send(command[6..]);
                }
            }
            Console.WriteLine("closing application ...");
        }
    }
}
