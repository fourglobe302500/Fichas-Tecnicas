using System;

using CLI.Processing;
using CLI.Utils;

namespace CLI
{
    class Program : Process
    {
        static void Main( )
        {
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.Write($"{_user ?? ""}{(_user == null || _entity == null ? "" : " ")}{_entity ?? ""}> ");
                Console.ForegroundColor = ConsoleColor.White;
                var line = Console.ReadLine();
                Console.ForegroundColor = ConsoleColor.DarkBlue;
                var reply = ProcessLine(line);
                if (reply.ShoudClose) return;
                if (reply.Errors != null) reply.Errors.ForEach((err) => Console.WriteLine(err));
            }
        }
    }
}