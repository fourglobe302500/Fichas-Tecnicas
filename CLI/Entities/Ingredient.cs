using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using CLI.Entities.Structs;
using CLI.Utilities;

namespace CLI.Entities
{
    internal sealed class Ingredient
    {
        private const string _filePath = "C:\\Data\\Fichas_Tecnicas\\Ingredientes.csv";

        private Ingredient(string name, float rendimento, float preçoPorKilo)
        {
            Name = name;
            Rendimento = rendimento;
            Price = preçoPorKilo;
        }

        public string Name { get; }
        public float Rendimento { get; }
        public float Price { get; }

        #region Creating

        internal static void Create(string[] args, ref List<string> errors, out IEnumerable<string> Data)
        {
            Data = null;
            var newIngredient = Construct(args, ref errors);
            if (errors.Any())
                return;

            var processedData = LoadData();

            if (processedData.Any((Ingrediente) => Ingrediente.name == newIngredient.Name))
            {
                errors.Add($"Ingrediente {newIngredient.Name} já existe");
                return;
            }
            var id = processedData.OrderByDescending(ingrediente => ingrediente.id).FirstOrDefault().id;
            Write(processedData.Append(new IngredientStruct(id + 1, newIngredient.Name, newIngredient.Rendimento, newIngredient.Price))
                .Select(ingredient => ingredient.ToString())
                .ToArray());
            Data = new List<string>() { (id + 1).ToString(), newIngredient.Name, newIngredient.Rendimento.ToString(), newIngredient.Price.ToString() };
            Console.WriteLine($"id: {id + 1}, Ingrediente: {newIngredient.Name}, Rendimento: {newIngredient.Rendimento}, Preço/Kg: {newIngredient.Price}");
        }

        #endregion Creating

        #region Reading

        internal static void Select(out IEnumerable<string> Data)
        {
            Data = null;
            Data = LoadData().SelectMany(ingredient => new string[] {
                ingredient.id.ToString(),
                ingredient.name,
                ingredient.rendimento.ToString(),
                ingredient.price.ToString() });
            Data.PrettyPrint(new string[] { "id", "Ingrediente", "Rendimento", "Preço" });
        }

        internal static void Select(string index, ref List<string> errors, out IEnumerable<string> Data)
        {
            Data = null;
            if (int.TryParse(index, out var Index))
            {
                Data = LoadData().Where(ingredient => ingredient.id == Index)
                                 .SelectMany(ingredient => new string[] {
                                     ingredient.id.ToString(),
                                     ingredient.name,
                                     ingredient.rendimento.ToString(),
                                     ingredient.price.ToString() });
                Data.PrettyPrint(new string[] { "id", "Ingrediente", "Rendimento", "Preço" });
            }
            else
            {
                errors.Add($"Numero integral 'index' em formato invalido");
            }
        }

        internal static void Select(string[] args, ref List<string> errors, out IEnumerable<string> Data)
        {
            Data = null;
            var data = LoadData();
            IEnumerable<IngredientStruct> sorted = null;
            switch (args[0])
            {
                case "nome":
                case "name":
                    sorted = data.Where(ingredient => ingredient.name == args[1]);
                    break;
                case "rendimento":
                    if (float.TryParse(args[1].Replace('.', ','), out var rend))
                        sorted = data.Where(ingredient => ingredient.rendimento == rend);
                    else
                        errors.Add("Numero 'Rendimento' em formato invalido");
                    break;
                case "preço":
                case "price":
                    if (float.TryParse(args[1].Replace('.', ','), out var price))
                        sorted = data.Where(ingredient => ingredient.price == price);
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
                errors.Add("Erro interno reporte erro 1404");
                return;
            }
            Data = sorted.SelectMany(ingredient => new string[] {
                ingredient.id.ToString(),
                ingredient.name,
                ingredient.rendimento.ToString(),
                ingredient.price.ToString() });
            Data.PrettyPrint(new string[] { "id", "Ingrediente", "Rendimento", "Preço" });
        }

        internal static bool SelectI(int index, ref List<string> errors, out IngredientStruct Data)
        {
            var IData = LoadData().Where(ingredient => ingredient.id == index);
            if (IData.Any())
            {
                Data = IData.Single();
                return true;
            }
            else
            {
                Data = new IngredientStruct();
                errors.Add($"Ingrediente de index {index} não existe.");
                return false;
            }
        }

        #endregion Reading

        #region Updating

        internal static void Update(string[] args, ref List<string> errors, out IEnumerable<string> Data)
        {
            Data = null;
            var upIngredient = Construct(args.Skip(1).ToArray(), ref errors);
            if (errors.Any())
                return;

            var processedData = LoadData();

            if (int.TryParse(args[0], out var id) && !processedData.Any((ingrediente) => ingrediente.id == id))
            {
                errors.Add($"Ingrediente de id {args[0]} não existe");
                return;
            }
            Write(processedData.Select(
                    ingredient => ingredient.id != id
                    ? ingredient.ToString()
                    : new IngredientStruct(ingredient.id, upIngredient.Name, upIngredient.Rendimento, upIngredient.Price).ToString())
                .ToArray());
            Data = new List<string>() { id.ToString(), upIngredient.Name, upIngredient.Rendimento.ToString(), upIngredient.Price.ToString() };
            Console.WriteLine($"id: {id}, Ingrediente: {upIngredient.Name}, Rendimento: {upIngredient.Rendimento}, Preço/Kg: {upIngredient.Price}");
        }

        #endregion Updating

        #region Deleting

        internal static void Delete(string index, ref List<string> errors)
        {
            if (int.TryParse(index, out var id))
            {
                var processedData = LoadData();
                if (processedData.Any((Ingrediente) => Ingrediente.id == id))
                    Write(processedData.Where(ingredient => ingredient.id != id).Select(ingrediente => ingrediente.ToString()).ToArray());
                else
                    errors.AddRange(new string[] { $"Ingrediente {id} não existe", "Incapaz de deletar o ingrediente por razões internas" });
            }
            else
            {
                errors.Add("Numero integral 'index' em formato invalido");
            }
        }

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

        #endregion Deleting

        #region Utilities

        private static Ingredient Construct(string[] args, ref List<string> errors)
        {
            var name = args[0];
            if (!float.TryParse(args[1].Replace('.', ','), out var rendimento))
            {
                errors.Add($"Rendimento em fomato invalido {args[1]}");
            }
            if (!float.TryParse(args[2].Replace('.', ','), out var price))
            {
                errors.Add($"Preço em fomato invalido {args[2]}");
            }
            return new Ingredient(name, rendimento, price);
        }

        private static void Write(string[] vals) => File.WriteAllLines(_filePath, vals);

        private static IngredientStruct[] LoadData( )
        {
            if (!File.Exists(_filePath))
                File.Create(_filePath).Close();

            var data = File.ReadAllLines(_filePath);
            return data.Select(line => line.Split(','))
                .Select((vals) => new IngredientStruct(int.Parse(vals[0]), vals[1], float.Parse(vals[2].Replace('.', ',')), float.Parse(vals[3].Replace('.', ','))))
                .ToArray();
        }

        #endregion Utilities
    }
}