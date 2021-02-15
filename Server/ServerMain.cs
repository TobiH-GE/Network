using System;
using System.Threading.Tasks;

namespace Server
{
    enum commands
    {
        start,
        stop,
        status,
        exit
    }
    class ServerMain
    {
        static void Main(string[] args)
        {

            ServerLogic Logic = new ServerLogic(ConsolePrint);
            Console.WriteLine("server starting ...");
            Start(Logic);
            WaitForCommands(Logic);
        }
        static async void Start(ServerLogic ServerLogic)
        {
            await Task.Run(() => ServerLogic.Start());
        }
        static void WaitForCommands(ServerLogic ServerLogic)
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
                    ServerLogic.SendAll(command[5..]);
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
