using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using CLI.Entities.Structs;
using CLI.Utilities;

namespace CLI.Entities
{
    internal sealed class Recipe
    {
        private const string _filePath = "C:\\Data\\Fichas_Tecnicas\\Recipes.csv";

        public Recipe(string recipeName, Link[] ingredients)
        {
            Name = recipeName;
            Ingredients = ingredients;
        }

        public string Name { get; }
        public Link[] Ingredients { get; }

        #region Creating

        internal static void Create(string[] args, ref List<string> errors, out IEnumerable<string> Data)
        {
            Data = null;
            var newRecipe = Construct(args);
            if (errors.Any())
                return;

            var processedData = LoadData();

            if (processedData.Any((Recipe) => Recipe.name == newRecipe.Name))
            {
                errors.Add($"Ingrediente {newRecipe.Name} já existe");
                return;
            }
            var id = processedData.OrderByDescending(recipe => recipe.id).FirstOrDefault().id;
            Write(processedData.Append(new RecipeStruct(id + 1, newRecipe.Name, newRecipe.Ingredients))
                .Select(recipe => recipe.ToString())
                .ToArray());
            Data = new List<string>() { (id + 1).ToString(), newRecipe.Name, newRecipe.Ingredients.ToArrayString() };
            Console.WriteLine($"id: {id + 1}, Receita: {newRecipe.Name}, Links: {newRecipe.Ingredients.ToArrayString()}");
        }

        #endregion Creating

        #region Reading

        internal static void Select(out IEnumerable<string> Data)
        {
            Data = LoadData().SelectMany(recipe => new string[] {
                        recipe.id.ToString(), recipe.name });
            Data.PrettyPrint(new string[] { "id", "Recipe" });
        }

        internal static void Select(string id, ref List<string> errors, out RecipeStruct Data)
        {
            if (int.TryParse(id, out var index))
            {
                var error = new List<string>();
                Data = LoadData().Where(recipe => recipe.id == index).SingleOrDefault();
                if (Processing.Process.showData)
                    Utils.WriteValues("Dados", (Data.id, "index"), (Data.name, "name"));
                Data.links
                    .OrderBy(link => link.ingredientId)
                    .Select<Link, (Link link, IngredientStruct ingredient)>(
                        link => (link,
                            Ingredient.SelectI(link.ingredientId, ref error, out var ingredient) ? ingredient : new IngredientStruct()))
                    .SelectMany(link => new string[] {
                        link.link.ingredientId.ToString(),
                        link.ingredient.name,
                        link.link.quantity.ToString(),
                        (1 / link.ingredient.rendimento).ToString(),
                        link.ingredient.rendimento.ToString(),
                        (link.link.quantity / (1 / link.ingredient.rendimento)).ToString(),
                        link.link.price.ToString(),
                        (link.link.quantity / (1 / link.ingredient.rendimento) * link.link.price).ToString()
                    })
                    .PrettyPrint(new string[] { "index", "Nome", "Qtdd Liq.", "FC", "Rend", "Qtdd bruta", "Unitario", "Preço bruto" }, "Ingredientes");
                errors.AddRange(error);
            }
            else
            {
                Data = new RecipeStruct();
                errors.Add("Numero integral index em formato invalido");
            }
        }

        //internal static void Select(string index, ref List<string> errors, out IEnumerable<string> Data)
        //{
        //    Data = null;
        //    if (int.TryParse(index, out var Index))
        //    {
        //        Data = LoadData().Where(ingredient => ingredient.id == Index)
        //                         .SelectMany(ingredient => new string[] {
        //                                     ingredient.id.ToString(),
        //                                     ingredient.name,
        //                                     ingredient.rendimento.ToString(),
        //                                     ingredient.price.ToString() });
        //        Data.PrettyPrint(new string[] { "id", "Ingrediente", "Rendimento", "Preço" });
        //    }
        //    else
        //    {
        //        errors.Add($"Numero integral 'index' em formato invalido");
        //    }
        //}

        //internal static void Select(string[] args, ref List<string> errors, out IEnumerable<string> Data)
        //{
        //    Data = null;
        //    var data = LoadData();
        //    IEnumerable<IngredientStruct> sorted = null;
        //    switch (args[0])
        //    {
        //        case "nome":
        //        case "name":
        //            sorted = data.Where(ingredient => ingredient.name == args[1]);
        //            break;
        //        case "rendimento":
        //            if (float.TryParse(args[1].Replace('.', ','), out var rend))
        //                sorted = data.Where(ingredient => ingredient.rendimento == rend);
        //            else
        //                errors.Add("Numero 'Rendimento' em formato invalido");
        //            break;
        //        case "preço":
        //        case "price":
        //            if (float.TryParse(args[1].Replace('.', ','), out var price))
        //                sorted = data.Where(ingredient => ingredient.price == price);
        //            else
        //                errors.Add("Numero 'Preço' em formato invalido");
        //            break;
        //        default:
        //            errors.Add($"Campo '{args[0]}' desconhecido");
        //            break;
        //    }
        //    if (errors.Any())
        //        return;
        //    if (sorted == null)
        //    {
        //        errors.Add("Erro interno reporte erro 1404");
        //        return;
        //    }
        //    Data = sorted.SelectMany(ingredient => new string[] {
        //                ingredient.id.ToString(),
        //                ingredient.name,
        //                ingredient.rendimento.ToString(),
        //                ingredient.price.ToString() });
        //    Data.PrettyPrint(new string[] { "id", "Ingrediente", "Rendimento", "Preço" });
        //}

        #endregion Reading

        #region Updating

        internal static void Update(string[] args, ref List<string> errors, out RecipeStruct Data)
        {
            Data = new RecipeStruct();
            var newRecipe = Construct(args.Skip(1).ToArray());
            if (errors.Any())
                return;

            var processedData = LoadData();

            if (int.TryParse(args[0], out var id) && !processedData.Any((recipe) => recipe.id == id))
            {
                errors.Add($"Receita de index {args[0]} não existe");
                return;
            }

            Write(
                processedData.Select(
                    recipe => recipe.id != id
                        ? recipe.ToString()
                        : new RecipeStruct(recipe.id, args[1], recipe.links).ToString())
                .ToArray());
            Data = LoadData().Where(recipe => recipe.id == id).Single();
            var error = new List<string>();
            if (Processing.Process.showData)
                Utils.WriteValues("New Recipe", (Data.id, "index"), (Data.name, "name"));
            Data.links
                .OrderBy(link => link.ingredientId)
                .Select<Link, (Link link, IngredientStruct ingredient)>(
                    link => (link,
                        Ingredient.SelectI(link.ingredientId, ref error, out var ingredient) ? ingredient : new IngredientStruct()))
                .SelectMany(link => new string[] {
                        link.link.ingredientId.ToString(),
                        link.ingredient.name,
                        link.link.quantity.ToString(),
                        (1 / link.ingredient.rendimento).ToString(),
                        link.ingredient.rendimento.ToString(),
                        (link.link.quantity / (1 / link.ingredient.rendimento)).ToString(),
                        link.link.price.ToString(),
                        (link.link.quantity / (1 / link.ingredient.rendimento) * link.link.price).ToString()
                })
                .PrettyPrint(new string[] { "index", "Nome", "Qtdd Liq.", "FC", "Rend", "Qtdd bruta", "Unitario", "Preço bruto" }, "Links");
            errors.AddRange(error);
        }

        #endregion Updating

        #region Deleting

        internal static void Delete(string index, ref List<string> errors)
        {
            if (int.TryParse(index, out var id))
            {
                var processedData = LoadData();
                if (processedData.Any((recipe) => recipe.id == id))
                    Write(processedData.Where(recipe => recipe.id != id).Select(recipe => recipe.ToString()).ToArray());
                else
                    errors.AddRange(new string[] { $"Receita {id} não existe", "Incapaz de deletar a receita por razões internas" });
            }
            else
            {
                errors.Add("Numero integral 'index' em formato invalido");
            }
        }
        /* TODO
                internal static void Delete(string[] args, ref List<string> errors)
                {
                    var data = LoadData();
                    IEnumerable<IngredientStruct> sorted = null;
                    switch (args[0])
                    {
                        case "nome":
                        case "name":
                            sorted = data.Where(ingredient => ingredient.name != args[1]);
                            break;
                        case "rendimento":
                            if (float.TryParse(args[1].Replace('.', ','), out var rend))
                                sorted = data.Where(ingredient => ingredient.rendimento != rend);
                            else
                                errors.Add("Numero 'Rendimento' em formato invalido");
                            break;
                        case "preço":
                        case "price":
                            if (float.TryParse(args[1].Replace('.', ','), out var price))
                                sorted = data.Where(ingredient => ingredient.price != price);
                            else
                                errors.Add("Numero 'Preço' em formato invalido");
                            break;
                        default:
                            errors.Add($"Campo '{args[0]}' desconhecido");
                            break;
                    }
                    if (errors.Any())
                        return;
                    if (sorted == null)
                    {
                        errors.Add("Erro interno reporte erro 3404");
                        return;
                    }
                    Write(sorted.Select(ingredient => ingredient.ToString()).ToArray());
                }

        */

        #endregion Deleting

        #region Utilities

        private static Recipe Construct(string[] args) => new Recipe(args.Single(), null);

        private static Link Link(string[] args, ref List<string> errors)
        {
            if (!int.TryParse(args[0], out var id))
                errors.Add($"Index em fomato invalido {args[0]}");
            if (!float.TryParse(args[1].Replace('.', ','), out var rendimento))
                errors.Add($"Rendimento em fomato invalido {args[1]}");
            if (!float.TryParse(args[2].Replace('.', ','), out var price))
                errors.Add($"Preço em fomato invalido {args[2]}");
            return (id, rendimento, price);
        }

        private static void Write(string[] vals) => File.WriteAllLines(_filePath, vals);

        private static RecipeStruct[] LoadData( )
        {
            if (!File.Exists(_filePath))
                File.Create(_filePath).Close();

            return File.ReadAllLines(_filePath)
                       .Select(line => line.Split(','))
                       .Select((vals) => new RecipeStruct(int.Parse(vals[0]), vals[1], Parse(vals[2])))
                       .ToArray();
        }

        private static Link[] Parse(string vals)
        {
            List<Link> result = new List<Link>();
            for (var pos = 0; pos < vals.Length; pos++)
            {
                Link current = (0, 0, 0);
                switch (vals[pos])
                {
                    case '\0':
                    case '[':
                    case ']':
                    case '{':
                    case '}':
                    case ';':
                        continue;
                    default:
                        string changing = "";
                        int last = 0;
                        while (vals[pos] != '}')
                        {
                            if (vals[pos] == ':')
                            {
                                current = (
                                    last == 0 ? int.Parse(changing) : current.ingredientId,
                                    last == 1 ? float.Parse(changing.Replace('.', ',')) : current.quantity,
                                    0);
                                last++;
                                changing = "";
                            }
                            else
                            {
                                changing += vals[pos];
                            }
                            pos++;
                        }
                        current = (current.ingredientId, current.quantity, float.Parse(changing.Replace('.', ',')));
                        result.Add(current);
                        continue;
                }
            }
            return result.ToArray();
        }

        #endregion Utilities
    }
}