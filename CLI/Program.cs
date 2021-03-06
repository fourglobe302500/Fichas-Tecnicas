﻿using System;

using CLI.Processing;
using CLI.Utilities;

namespace CLI
{
    class Program : Process
    {
        static void Main()
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("For help enter '\\?'\n");
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.Write($"{_user ?? ""}{(_user == null || _entity == null ? "" : " ")}{_entity ?? ""}> ");
                Console.ForegroundColor = ConsoleColor.White;
                var line = Console.ReadLine();
                Console.ForegroundColor = ConsoleColor.DarkBlue;
                var reply = ProcessLine(line);
                if (reply.ShoudClose) return;
                Console.ForegroundColor = ConsoleColor.Red;
                if (reply.Errors != null) reply.Errors.ForEach((err) => Console.WriteLine(err));
            }
        }
    }
}