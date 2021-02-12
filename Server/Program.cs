using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using ServerLogic;

namespace Server
{
    enum commands
    {
        start,
        stop,
        status,
        exit
    }
    class Program
    {
        static void Main(string[] args)
        {

            Logic ServerLogic = new Logic(ConsolePrint);
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
            while (command != commands.exit.ToString())
            {
                command = Console.ReadLine()[1..];
                if (command == "stop")
                {
                    ServerLogic.Stop();
                }
                if (command == "status")
                {
                    ServerLogic.Status();
                }
                if (command == "send")
                {
                    ServerLogic.Send(command[5..]);
                }
            }
            Console.WriteLine("closing application ...");
        }
        static void ConsolePrint(string msg)
        {
            Console.WriteLine(msg);
        }
    }
}
