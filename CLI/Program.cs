using System;
using System.Collections.Generic;
using System.Linq;

using CLI.Entities;
using CLI.Utils;

namespace CLI
{
    class Program
    {
        private static string _user = null;
        private static string _entity = null;

        static void Main( )
        {
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.Write($"${(_user ?? "")}{(_entity ?? "")}> ");
                Console.ForegroundColor = ConsoleColor.White;
                var line = Console.ReadLine();
                Console.ForegroundColor = ConsoleColor.DarkBlue;
                if (string.IsNullOrWhiteSpace(line))
                    continue;
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
                    case "use":
                        if (ValidateArguments(statements, 1))
                        {
                            _entity = null;
                        }
                        else if (!ValidateArguments(statements, 2))
                        {
                            Console.WriteLine("Argumentos invalidos para use de tabela");
                        }
                        else
                        {
                            if ((new string[] { "ingredient", "ingrediente" }).Contains(statements[1]))
                                _entity = statements[1];
                            else
                                Console.WriteLine("Tabela desconhecida");
                        }
                        break;
                    default:
                        Console.WriteLine($"Token desconhecido '{statements[0]}'");
                        break;
                }
            }
        }

        private static void HandleSelect(string[] args)
        {
            if (!Valid(args))
                return;
            switch (_entity ?? args[0])
            {
                case "ingrediente":
                case "ingredient":
                    if (!ValidateArguments(args, _entity != null ? 0 : 1))
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
            if (!Valid(args))
                return;
            switch (_entity ?? args[0])
            {
                case "ingrediente":
                case "ingredient":
                    if (!ValidateArguments(args, _entity != null ? 3 : 4))
                        Console.WriteLine($"Numero de argumentos invalido");
                    else
                        if (!Ingrediente.Create(args.Skip(_entity != null ? 0 : 1).ToArray()).Save())
                        Console.WriteLine("Incapaz de criar o ingrediente por razões internals");
                    break;
                default:
                    Console.WriteLine($"Entidade desconhecida '{args[0]}'");
                    break;
            }
        }

        private static void HandleUpdate(string[] args)
        {
            if (!Valid(args))
                return;
            switch (_entity ?? args[0])
            {
                case "ingrediente":
                case "ingredient":
                    if (!ValidateArguments(args, _entity != null ? 3 : 4))
                        Console.WriteLine($"Numero de argumentos invalido");
                    else
                        if (!Ingrediente.Create(args.Skip(_entity != null ? 0 : 1).ToArray()).Update())
                        Console.WriteLine("Incapaz de atualizar o ingrediente por razões internals");
                    break;
                default:
                    Console.WriteLine($"Entidade desconhecida '{args[0]}'");
                    break;
            }
        }

        private static void HandleDelete(string[] args)
        {
            if (!Valid(args))
                return;
            switch (_entity ?? args[0])
            {
                case "ingrediente":
                case "ingredient":
                    if (!ValidateArguments(args, _entity != null ? 1 : 2))
                        Console.WriteLine($"Numero de argumentos invalido");
                    else
                        if (!Ingrediente.Delete(args[_entity != null ? 0 : 1]))
                        Console.WriteLine("Incapaz de deletar o ingrediente por razões internals");
                    break;
                default:
                    Console.WriteLine($"Entidade desconhecida '{args[0]}'");
                    break;
            }
        }

        private static bool ValidateArguments(string[] args, int index) => args.Length == index || string.IsNullOrWhiteSpace(args[index]);

        private static bool Valid(string[] args)
        {
            if (_entity != null)
            {
                return true;
            }
            if (ValidateArguments(args, 0))
            {
                Console.WriteLine("Faltando entidade para ser criada");
                return false;
            }
            return true;
        }
    }
}