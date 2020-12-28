using System;
using System.Collections.Generic;
using System.Linq;

using CLI.Entities;
using CLI.Utils;

namespace CLI
{
    class Program
    {
        static void Main( )
        {
            var user = "";
            var entity = "";
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            while (true)
            {
                Console.Write($"${user}{entity}>");
                Console.ForegroundColor = ConsoleColor.White;
                var line = Console.ReadLine();
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                if (string.IsNullOrWhiteSpace(line))
                    break;
                if (line.StartsWith('\\'))
                {
                    switch (line)
                    {
                        case "\\q":
                            return;
                        case "\\cls":
                            Console.Clear();
                            break;
                    }
                    continue;
                }
                var statements = line.ToLower().Split(' ');
                switch (statements[0])
                {
                    case "novo":
                    case "create":
                        HandleCreate(statements.Skip(1).ToArray());
                        break;
                    case "selecionar":
                    case "select":
                        HandleSelect(statements.Skip(1).ToArray());
                        break;
                    case "atualizar":
                    case "update":
                        HandleUpdate(statements.Skip(1).ToArray());
                        break;
                    case "deletar":
                    case "delete":
                        HandleDelete(statements.Skip(1).ToArray());
                        break;
                    default:
                        Console.WriteLine($"Token desconhecido '{statements[0]}'");
                        break;
                }
            }
        }

        private static void HandleSelect(string[] args)
        {
            if (validateArguments(args, 0))
            {
                Console.WriteLine("Faltando entidade para ser selecionada");
                return;
            }
            switch (args[0])
            {
                case "ingrediente":
                case "ingredient":
                    if (!validateArguments(args, 1))
                        Console.WriteLine($"Numero de argumentos invalido");
                    else
                        Ingrediente.Select();
                    break;
                default:
                    Console.WriteLine($"Entidade desconhecida '{args[0]}'");
                    break;
            }
        }

        private static void HandleCreate(string[] args)
        {
            if (validateArguments(args, 0))
            {
                Console.WriteLine("Faltando entidade para ser criada");
                return;
            }
            switch (args[0])
            {
                case "ingrediente":
                case "ingredient":
                    if (!validateArguments(args, 4))
                        Console.WriteLine($"Numero de argumentos invalido");
                    else
                        if (!Ingrediente.Create(args.Skip(1).ToArray()).Save())
                        Console.WriteLine("Incapaz de criar o ingrediente por razões internals");
                    break;
                default:
                    Console.WriteLine($"Entidade desconhecida '{args[0]}'");
                    break;
            }
        }

        private static void HandleUpdate(string[] args)
        {
            if (validateArguments(args, 0))
            {
                Console.WriteLine("Faltando entidade para ser atualizada");
                return;
            }
            switch (args[0])
            {
                case "ingrediente":
                case "ingredient":
                    if (!validateArguments(args, 4))
                        Console.WriteLine($"Numero de argumentos invalido");
                    else
                        if (!Ingrediente.Create(args.Skip(1).ToArray()).Update())
                        Console.WriteLine("Incapaz de atualizar o ingrediente por razões internals");
                    break;
                default:
                    Console.WriteLine($"Entidade desconhecida '{args[0]}'");
                    break;
            }
        }

        private static void HandleDelete(string[] args)
        {
            if (validateArguments(args, 0))
            {
                Console.WriteLine("Faltando entidade para ser delateda");
                return;
            }
            switch (args[0])
            {
                case "ingrediente":
                case "ingredient":
                    if (!validateArguments(args, 2))
                        Console.WriteLine($"Numero de argumentos invalido");
                    else
                        if (!Ingrediente.Delete(args[1]))
                        Console.WriteLine("Incapaz de deletar o ingrediente por razões internals");
                    break;
                default:
                    Console.WriteLine($"Entidade desconhecida '{args[0]}'");
                    break;
            }
        }

        private static bool validateArguments(string[] args, int index) => args.Length == index || string.IsNullOrWhiteSpace(args[index]);
    }
}