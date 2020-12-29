using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using CLI.Utils;

namespace CLI.Entities
{
    internal sealed class Ingrediente
    {
        private const string _filePath = "C:\\Data\\Fichas_Tecnicas\\Ingredientes.csv";

        private Ingrediente(string name, float rendimento, float preçoPorKilo)
        {
            Name = name;
            Rendimento = rendimento;
            Price = preçoPorKilo;
        }

        public string Name { get; }
        public float Rendimento { get; }
        public float Price { get; }

        internal static void Select( )
            => LoadData().ForEach((ingredient)
                => Console.WriteLine($"id: {ingredient.id}, Ingrediente: {ingredient.name}, Rendimento: {ingredient.rendimento}, Preço/Kg: {ingredient.price}"));

        internal static void Delete(string name, ref List<string> errors)
        {
            var processedData = LoadData();
            if (processedData.Any((Ingrediente) => Ingrediente.name == name))
                Write(processedData.Where(ingredient => ingredient.name != name).Select(ingrediente => ingrediente.ToString()).ToArray());
            else
                errors.AddRange(new string[] { $"Ingrediente {name} não existe", "Incapaz de deletar o ingrediente por razões privates" });
        }

        internal static void Update(string[] args, ref List<string> errors)
        {
            var upIngredient = Construct(args, ref errors);
            if (errors.Any())
                return;

            var processedData = LoadData();

            if (!processedData.Any((ingrediente) => ingrediente.name == upIngredient.Name))
            {
                errors.Add($"Ingrediente {upIngredient.Name} não existe");
                return;
            }
            Write(processedData.Select(
                    ingredient => ingredient.name != upIngredient.Name
                    ? ingredient.ToString()
                    : new IngredientStruct(ingredient.id, upIngredient.Name, upIngredient.Rendimento, upIngredient.Price).ToString())
                .ToArray());
            var id = processedData.Where(Ingrediente => Ingrediente.name == upIngredient.Name).Single().id;
            Console.WriteLine($"id: {id}, Ingrediente: {upIngredient.Name}, Rendimento: {upIngredient.Rendimento}, Preço/Kg: {upIngredient.Price}");
        }

        internal static void Create(string[] args, ref List<string> errors)
        {
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
            Console.WriteLine($"id: {id + 1}, Ingrediente: {newIngredient.Name}, Rendimento: {newIngredient.Rendimento}, Preço/Kg: {newIngredient.Price}");
        }

        private static Ingrediente Construct(string[] args, ref List<string> errors)
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
            return new Ingrediente(name, rendimento, price);
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
    }
}