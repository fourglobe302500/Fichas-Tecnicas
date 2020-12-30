using System;
using System.Collections.Generic;
using System.Linq;

using CLI.Entities;

namespace CLI.Processing
{
    internal class Process
    {
        private protected static string _user = null;
        private protected static string _entity = null;

        #region Processing

        private protected struct ProcessReturn
        {
            public bool ShoudClose { get; private set; }
            public string[] Errors { get; private set; }

            public static ProcessReturn FromErrors(string[] errors) => new ProcessReturn() { Errors = errors };

            public static ProcessReturn FromShouldClose(bool shouldClose) => new ProcessReturn() { ShoudClose = shouldClose };
        }

        private protected static ProcessReturn ProcessLine(string line) => string.IsNullOrWhiteSpace(line)
                ? new ProcessReturn()
                : line.StartsWith('\\') ? ProcessMeta(line) : ProcessStatement(line.ToLower().Split(' '));

        private static ProcessReturn ProcessMeta(string line)
        {
            var shouldClose = false;
            switch (line)
            {
                case "\\q":
                    shouldClose = true;
                    break;
                case "\\cls":
                    Console.Clear();
                    break;
            }
            return ProcessReturn.FromShouldClose(shouldClose);
        }

        private static ProcessReturn ProcessStatement(string[] statements)
        {
            List<string> errors = new List<string>();
            switch (statements[0])
            {
                case "novo":
                case "create":
                    return HandleCreate(statements.Skip(1).ToArray());
                case "selecionar":
                case "select":
                    return HandleSelect(statements.Skip(1).ToArray());
                case "atualizar":
                case "update":
                    return HandleUpdate(statements.Skip(1).ToArray());
                case "deletar":
                case "delete":
                    return HandleDelete(statements.Skip(1).ToArray());
                case "use":
                    return HandleUse(statements.Skip(1).ToArray());
                default:
                    errors.Add($"Token desconhecido '{statements[0]}'");
                    break;
            }
            return ProcessReturn.FromErrors(errors.ToArray());
        }

        #endregion Processing

        #region Handlers

        private static ProcessReturn HandleCreate(string[] args)
        {
            if (!Valid(args))
                return ProcessReturn.FromErrors(new string[] { "Faltando entidade para ser atualizada" });
            List<string> errors = new List<string>();
            switch (_entity ?? args[0])
            {
                case "ingrediente":
                case "ingredient":
                    if (!ValidateArguments(args, _entity != null ? 3 : 4))
                        errors.Add($"Numero de argumentos invalido");
                    else
                        Ingrediente.Create(args.Skip(_entity != null ? 0 : 1).ToArray(), ref errors);
                    break;
                default:
                    errors.Add($"Entidade desconhecida '{args[0]}'");
                    break;
            }
            return ProcessReturn.FromErrors(errors.ToArray());
        }

        private static ProcessReturn HandleSelect(string[] args)
        {
            if (!Valid(args))
                return ProcessReturn.FromErrors(new string[] { "Faltando entidade para ser atualizada" });
            List<string> errors = new List<string>();
            switch (_entity ?? args[0])
            {
                case "ingrediente":
                case "ingredient":
                    if (!ValidateArguments(args, _entity != null ? 0 : 1))
                        errors.Add($"Numero de argumentos invalido");
                    else
                        Ingrediente.Select();
                    break;
                default:
                    errors.Add($"Entidade desconhecida '{args[0]}'");
                    break;
            }
            return ProcessReturn.FromErrors(errors.ToArray());
        }

        private static ProcessReturn HandleUpdate(string[] args)
        {
            if (!Valid(args))
                return ProcessReturn.FromErrors(new string[] { "Faltando entidade para ser atualizada" });
            List<string> errors = new List<string>();
            switch (_entity ?? args[0])
            {
                case "ingrediente":
                case "ingredient":
                    if (!ValidateArguments(args, _entity != null ? 3 : 4))
                        errors.Add($"Numero de argumentos invalido");
                    else
                        Ingrediente.Update(args.Skip(_entity != null ? 0 : 1).ToArray(), ref errors);
                    break;
                default:
                    errors.Add($"Entidade desconhecida '{args[0]}'");
                    break;
            }
            return ProcessReturn.FromErrors(errors.ToArray());
        }

        private static ProcessReturn HandleDelete(string[] args)
        {
            if (!Valid(args))
                return ProcessReturn.FromErrors(new string[] { "Faltando entidade para ser deletada" });
            List<string> errors = new List<string>();
            switch (_entity ?? args[0])
            {
                case "ingrediente":
                case "ingredient":
                    if (!ValidateArguments(args, _entity != null ? 1 : 2))
                        errors.Add($"Numero de argumentos invalido");
                    else
                        Ingrediente.Delete(args[_entity != null ? 0 : 1], ref errors);
                    break;
                default:
                    errors.Add($"Entidade desconhecida '{args[0]}'");
                    break;
            }
            return ProcessReturn.FromErrors(errors.ToArray());
        }

        private static ProcessReturn HandleUse(string[] args)
        {
            List<string> errors = new List<string>();
            if (ValidateArguments(args, 0))
            {
                _entity = null;
            }
            else if (!ValidateArguments(args, 1))
            {
                errors.Add("Argumentos invalidos para use de tabela");
            }
            else
            {
                if ((new string[] { "ingredient", "ingrediente" }).Contains(args[0]))
                    _entity = args[0];
                else
                    errors.Add("Tabela desconhecida");
            }
            return ProcessReturn.FromErrors(errors.ToArray());
        }

        #endregion Handlers

        #region Validation

        private static bool Valid(string[] args) => _entity != null || !ValidateArguments(args, 0);

        private static bool ValidateArguments(string[] args, int index) => args.Length == index
                ? index == 0 || !string.IsNullOrWhiteSpace(args[index - 1])
                : index <= args.Length && string.IsNullOrWhiteSpace(args[index]);

        #endregion Validation
    }
}