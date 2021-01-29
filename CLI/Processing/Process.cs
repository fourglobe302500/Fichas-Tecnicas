using System;
using System.Collections.Generic;
using System.Linq;

using CLI.Entities;

namespace CLI.Processing
{
    internal class Process
    {
        public static bool showData = true;
        private protected static string _user = null;
        private protected static string _entity = null;

        #region Processing

        private protected struct ProcessReturn
        {
            public bool ShoudClose { get; private set; }
            public string[] Errors { get; private set; }

            private ProcessReturn(bool shouldClose, string[] errors)
            {
                ShoudClose = shouldClose;
                Errors = errors;
            }
            public static ProcessReturn Null => new ProcessReturn(false, null);

            public static ProcessReturn FromErrors(string[] errors) => new ProcessReturn(false, errors);

            public static ProcessReturn FromShouldClose(bool shouldClose) => new ProcessReturn(shouldClose, null);
        }

        private protected static ProcessReturn ProcessLine(string line) => string.IsNullOrWhiteSpace(line)
                ? ProcessReturn.Null
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
                case "\\noData":
                    showData = !showData;
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"{(!showData ? "not " : "")}showing data");
                    break;
                case "\\?":
                case "\\h":
                case "\\help":
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write(Environment.NewLine + new string('-', Console.WindowWidth * 2));
                    Console.Write(@"
Help Menu
To detailed help enter: \help <command>

select, selecionar:

    Selects the data of an object from a table.

create, novo:

    Crates a new data object in a table.

update, atualizar:

    Update the data of an object of a table.

delete, deletar:

    Delete an object of a table.

link, linkar:

    Links a object from a table to a object of another table.

unlink, deslinkar:

    Unlinks a object from a table to a object of another table.

use:

    Syntax sugar for table selection.

\q:

    Quits the application.

\cls:

    Clears the screen.

\noData:

    Omits the header of recipes selection.

");
                    break;
                case "\\help select":
                case "\\help selecionar":
                case "\\h select":
                case "\\h selecionar":
                case "\\? select":
                case "\\? selecionar":
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write(Environment.NewLine + new string('-', Console.WindowWidth * 2));
                    Console.Write(@"
Help Menu

    Syntax:
        [select | selecionar] *table

            Selects all data of a table**.

        [select | selecionar] *table <id>

            Selects all data of a object in a table of matching id.

        [select | selecionar] *table <key> <value>

            Select all data of all matching object where the value of the key atribute is equals to the value parameter***.

    * Not necessary when using use syntax.

    ** Will not select array or linked object data.

    *** Currently does this syntax doesn't work with the recipes table.

");
                    break;
                case "\\help create":
                case "\\help novo":
                case "\\h create":
                case "\\h novo":
                case "\\? create":
                case "\\? novo":
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write(Environment.NewLine + new string('-', Console.WindowWidth * 2));
                    Console.Write(@"
Help Menu

    Syntax:
        [create | novo] *table <values>

            Creates a new object in the table with the values.

    * Not necessary when using use syntax.

    Values order:

        ingredient: <nome: string> <rendimento: float> <preço: float>
        recipe: <nome: string> <rendimento: float>

");
                    break;
                case "\\help update":
                case "\\help atualizar":
                case "\\h update":
                case "\\h atualizar":
                case "\\? update":
                case "\\? atualizar":
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write(Environment.NewLine + new string('-', Console.WindowWidth * 2));
                    Console.Write(@"
Help Menu

    Syntax:
        [update | atualizar] *table <id> <values>

            Updates the object of id data to the new values.

    * Not necessary when using use syntax.

    Values order:

        ingredient: <nome: string> <rendimento: float> <preço: float>
        recipe: <nome: string> <rendimento: float>

");
                    break;
                case "\\help delete":
                case "\\help deletar":
                case "\\h delete":
                case "\\h deletar":
                case "\\? delete":
                case "\\? deletar":
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write(Environment.NewLine + new string('-', Console.WindowWidth * 2));
                    Console.Write(@"
Help Menu

    Syntax:
        [delete | deletar] *table <id>

            Deletes the object of id.

    * Not necessary when using use syntax.

");
                    break;
                case "\\help link":
                case "\\help linkar":
                case "\\h link":
                case "\\h linkar":
                case "\\? link":
                case "\\? linkar":
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write(Environment.NewLine + new string('-', Console.WindowWidth * 2));
                    Console.Write(@"
Help Menu

    Syntax:
        [link | linkar] *recipe <id> <values>

            Makes a link of a ingredient with a recipe.

    * Not necessary when using use syntax.

    Values order:

        <id: int> <quantidade: float> <preço: float>

");
                    break;
                case "\\help unlink":
                case "\\help deslinkar":
                case "\\h unlink":
                case "\\h deslinkar":
                case "\\? unlink":
                case "\\? deslinkar":
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write(Environment.NewLine + new string('-', Console.WindowWidth * 2));
                    Console.Write(@"
Help Menu

    Syntax:
        [unlink | deslinkar] *recipe <id1> <id2>

            Unlinks the ingredient of id2 from the recipe of id1.

    * Not necessary when using use syntax.

");
                    break;
                case "\\help use":
                case "\\h use":
                case "\\? use":
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write(Environment.NewLine + new string('-', Console.WindowWidth * 2));
                    Console.Write(@"
Help Menu

    Syntax:
        use table

            Autofill the table option of all commands making then unnecessary, but with one table selected you can't use the command to read other.
            Enter '\use' to nullify the selection of table.

");
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
                case "link":
                case "linkar":
                    return HandleLink(statements.Skip(1).ToArray());
                case "unlink":
                case "deslinkar":
                    return HandleUnlink(statements.Skip(1).ToArray());
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
                    if (ValidateArguments(args, _entity != null ? 3 : 4))
                        Ingredient.Create(args.Skip(_entity != null ? 0 : 1).ToArray(), ref errors, out var _);
                    else
                        errors.Add($"Numero de argumentos invalido");
                    break;
                case "recipe":
                case "receita":
                    if (ValidateArguments(args, _entity != null ? 2 : 3))
                        Recipe.Create(args.Skip(_entity != null ? 0 : 1).ToArray(), ref errors, out var _);
                    else
                        errors.Add($"Numero de argumentos invalido");
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
                    if (ValidateArguments(args, _entity != null ? 0 : 1))
                        Ingredient.Select(out var _);
                    else if (ValidateArguments(args, _entity != null ? 1 : 2))
                        Ingredient.Select(args[_entity != null ? 0 : 1], ref errors, out var _);
                    else if (ValidateArguments(args, _entity != null ? 2 : 3))
                        Ingredient.Select(_entity != null ? args : args.Skip(1).ToArray(), ref errors, out var _);
                    else
                        errors.Add($"Numero de argumentos invalido");
                    break;
                case "recipe":
                case "receita":
                    if (ValidateArguments(args, _entity != null ? 0 : 1))
                        Recipe.Select(out var _);
                    else if (ValidateArguments(args, _entity != null ? 1 : 2))
                        Recipe.Select(args[_entity != null ? 0 : 1], ref errors, out var _);
                    else
                        errors.Add($"Numero de argumentos invalido");
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
                    if (!ValidateArguments(args, _entity != null ? 4 : 5))
                        errors.Add($"Numero de argumentos invalido");
                    else
                        Ingredient.Update(args.Skip(_entity != null ? 0 : 1).ToArray(), ref errors, out var _);
                    break;
                case "recipe":
                case "receita":
                    if (ValidateArguments(args, _entity != null ? 3 : 4))
                        Recipe.Update(args.Skip(_entity != null ? 0 : 1).ToArray(), ref errors, out var _);
                    else
                        errors.Add($"Numero de argumentos invalido");
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
                    if (ValidateArguments(args, _entity != null ? 1 : 2))
                        Ingredient.Delete(args[_entity != null ? 0 : 1], ref errors);
                    else if (ValidateArguments(args, _entity != null ? 2 : 3))
                        Ingredient.Delete(_entity != null ? args : args.Skip(1).ToArray(), ref errors);
                    else
                        errors.Add($"Numero de argumentos invalido");
                    break;
                case "recipe":
                case "receita":
                    if (ValidateArguments(args, _entity != null ? 1 : 2))
                        Recipe.Delete(args[_entity != null ? 0 : 1], ref errors);
                    else
                        errors.Add("Numero de argumentos invalido");
                    break;
                default:
                    errors.Add($"Entidade desconhecida '{args[0]}'");
                    break;
            }
            return ProcessReturn.FromErrors(errors.ToArray());
        }

        private static ProcessReturn HandleLink(string[] args)
        {
            if (!Valid(args))
                return ProcessReturn.FromErrors(new string[] { "Faltanda entidade para ser linkada" });
            List<string> errors = new List<string>();
            switch (_entity ?? args[0])
            {
                case "recipe":
                case "receita":
                    if (ValidateArguments(args, _entity != null ? 3 : 4))
                        Recipe.Link(_entity != null ? args : args.Skip(1).ToArray(), ref errors, out var _);
                    else
                        errors.Add("Numero de argumentos invalido");
                    break;
            }
            return ProcessReturn.FromErrors(errors.ToArray());
        }

        private static ProcessReturn HandleUnlink(string[] args)
        {
            if (!Valid(args))
                return ProcessReturn.FromErrors(new string[] { "Faltanda entidade para ser deslinkada" });
            List<string> errors = new List<string>();
            switch (_entity ?? args[0])
            {
                case "recipe":
                case "receita":
                    if (ValidateArguments(args, _entity != null ? 2 : 3))
                        Recipe.Unlink(_entity != null ? args : args.Skip(1).ToArray(), ref errors);
                    else
                        errors.Add("Numero de argumentos invalido");
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
                if ((new string[] { "ingredient", "ingrediente", "recipe", "receita" }).Contains(args[0]))
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