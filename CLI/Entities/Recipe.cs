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

        public Recipe(string recipeName, float rendimento, Link[] ingredients)
        {
            Name = recipeName;
            Rendimento = rendimento;
            Ingredients = ingredients;
        }

        public string Name { get; }
        public float Rendimento { get; }
        public Link[] Ingredients { get; }

        #region Creating

        internal static void Create(string[] args, ref List<string> errors, out IEnumerable<string> Data)
        {
            Data = null;
            var newRecipe = Construct(args, ref errors);
            if (errors.Any())
                return;

            var processedData = LoadData();

            if (processedData.Any((Recipe) => Recipe.name == newRecipe.Name))
            {
                errors.Add($"Ingrediente {newRecipe.Name} já existe");
                return;
            }
            var id = processedData.OrderByDescending(recipe => recipe.id).FirstOrDefault().id;
            Write(processedData.Append(new RecipeStruct(id + 1, newRecipe.Name, newRecipe.Rendimento, newRecipe.Ingredients))
                .Select(recipe => recipe.ToString())
                .ToArray());
            Data = new List<string>() { (id + 1).ToString(), newRecipe.Name, newRecipe.Ingredients.ToArrayString() };
            Console.WriteLine($"id: {id + 1}, Receita: {newRecipe.Name}, Links: {newRecipe.Ingredients.ToArrayString()}");
        }

        #endregion Creating

        #region Reading

        #region public

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
                InternalSelect(index, ref errors, out Data);
            }
            else
            {
                Data = new RecipeStruct();
                errors.Add("Numero integral index em formato invalido");
            }
        }

        #endregion public

        #region internal

        internal static void InternalSelect(int index, ref List<string> errors, out RecipeStruct Data)
        {
            var data = LoadData().Where(recipe => recipe.id == index);
            if (!data.Any())
            {
                errors.Add($"Receita de index {index} não existe");
                Data = new RecipeStruct();
                return;
            }
            Data = data.SingleOrDefault();
            var error = new List<string>();
            var nData = Data.links
                .OrderBy(link => link.ingredientId)
                .Select<Link, (Link link, IngredientStruct ingredient)>(
                    link => (link,
                        Ingredient.SelectI(link.ingredientId, ref error, out var ingredient) ? ingredient : new IngredientStruct()));
            if (Processing.Process.showData)
            {
                float total = nData.Select((link) => link.link.quantity / (1 / link.ingredient.rendimento) * link.ingredient.price)
                                   .Aggregate(0, (float last, float val) => val + last, val => val);
                Utils.WriteValues("Dados",
                                  (Data.id, "index"),
                                  (Data.name, "name"),
                                  (Data.rendimento, "rendimento"),
                                  (total, "Preço total"),
                                  (MathF.Floor(Data.rendimento / .25f * 100) / 100, "Porções de 250g"),
                                  (MathF.Floor(total / (MathF.Floor(Data.rendimento / .25f * 100) / 100) * 100) / 100, "Preço/Porção 250g"),
                                  (MathF.Floor(Data.rendimento / .55f * 100) / 100, "Porções de 550g"),
                                  (MathF.Floor(total / (MathF.Floor(Data.rendimento / .55f * 100) / 100) * 100) / 100, "Preço/Porção 550g"));
            }

            nData.SelectMany(link => new string[] {
                        link.link.ingredientId.ToString(),
                        link.ingredient.name,
                        (MathF.Floor(link.link.quantity*100)/100).ToString(),
                        (MathF.Floor(1 / link.ingredient.rendimento*100)/100).ToString(),
                        link.ingredient.rendimento.ToString(),
                        (MathF.Floor(link.link.quantity / (1 / link.ingredient.rendimento)*100)/100).ToString(),
                        (MathF.Floor(link.ingredient.price * 100)/100).ToString(),
                        (MathF.Floor(link.link.quantity / (1 / link.ingredient.rendimento) * link.ingredient.price*100)/100).ToString()
                    })
                .PrettyPrint(new string[] { "index", "Nome", "Qtdd Liq.", "FC", "Rend", "Qtdd bruta", "Unitario", "Preço bruto" }, "Ingredientes");
        }

        #endregion internal

        #endregion Reading

        #region Updating

        internal static void Update(string[] args, ref List<string> errors, out RecipeStruct Data)
        {
            Data = new RecipeStruct();
            var newRecipe = Construct(args.Skip(1).ToArray(), ref errors);
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
                        : new RecipeStruct(recipe.id, args[1], newRecipe.Rendimento, recipe.links).ToString())
                .ToArray());
            InternalSelect(id, ref errors, out Data);
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

        #endregion Deleting

        #region Linking

        internal static void Link(string[] args, ref List<string> errors, out RecipeStruct Data)
        {
            Data = new RecipeStruct();
            if (!int.TryParse(args[0], out var id))
                errors.Add("Numero integral index em formato invalido");
            var link = Link(args[1..^0], ref errors);
            var processedData = LoadData();
            Write(processedData.Select(recipe => recipe.id == id
                    ? new RecipeStruct(id, recipe.name, recipe.rendimento, recipe.links.Append(link).ToArray()).ToString()
                    : recipe.ToString()).ToArray());
            if (errors.Any())
                return;
            Select(args[0], ref errors, out Data);
        }

        internal static void Unlink(string[] args, ref List<string> errors)
        {
            if (!int.TryParse(args[0], out var recId) || !int.TryParse(args[1], out var ingId))
            {
                errors.Add("Numero integral index em formato invalido");
                return;
            }
            var processedData = LoadData();
            if (!processedData.Any(recipe => recipe.id == recId && recipe.links.Any(link => link.ingredientId == ingId)))
            {
                errors.Add($"Link do ingredient {recId} e ingrediente {ingId} não existe");
                return;
            }
            Write(LoadData().Select(recipe => recipe.id == recId
                    ? new RecipeStruct(recId, recipe.name, recipe.rendimento, recipe.links.Where(link => link.ingredientId != ingId).ToArray()).ToString()
                    : recipe.ToString()).ToArray());
            Select(args[0], ref errors, out var _);
        }

        #endregion Linking

        #region Utilities

        private static Recipe Construct(string[] args, ref List<string> errors)
        {
            if (!float.TryParse(args[1].Replace('.', ','), out var rendimento))
            {
                errors.Add("Numero rendimento em formato invalido");
                return null;
            }
            return new Recipe(args[0], rendimento, null);
        }

        private static Link Link(string[] args, ref List<string> errors)
        {
            if (!int.TryParse(args[0], out var id))
                errors.Add($"Index em fomato invalido {args[0]}");
            if (!float.TryParse(args[1].Replace('.', ','), out var quantity))
                errors.Add($"Quantidade em fomato invalido {args[1]}");
            return new Link(id, quantity);
        }

        private static void Write(string[] vals) => File.WriteAllLines(_filePath, vals);

        private static RecipeStruct[] LoadData()
        {
            if (!File.Exists(_filePath))
                File.Create(_filePath).Close();

            return File.ReadAllLines(_filePath)
                       .Select(line => line.Split(','))
                       .Select((vals) => new RecipeStruct(int.Parse(vals[0]), vals[1], float.Parse(vals[2]), Parse(vals[3])))
                       .ToArray();
        }

        private static Link[] Parse(string vals)
        {
            List<Link> result = new List<Link>();
            for (var pos = 0; pos < vals.Length; pos++)
            {
                Link current = (0, 0);
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
                        while (vals[pos] != '}')
                        {
                            if (vals[pos] == ':')
                            {
                                current = (int.Parse(changing), 0);
                                changing = "";
                            }
                            else
                            {
                                changing += vals[pos];
                            }
                            pos++;
                        }
                        current = (current.ingredientId, float.Parse(changing.Replace('.', ',')));
                        result.Add(current);
                        continue;
                }
            }
            return result.ToArray();
        }

        #endregion Utilities
    }
}