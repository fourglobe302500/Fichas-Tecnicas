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

        private Ingrediente(string name, float rendimento, float preçoPorKilo, bool error)
        {
            Name = name;
            Rendimento = rendimento;
            Price = preçoPorKilo;
            Error = error;
        }

        public string Name { get; }
        public float Rendimento { get; }
        public float Price { get; }
        public bool Error { get; }

        internal static void Select( )
            => LoadData().ForEach((ingredient)
                => Console.WriteLine($"id: {ingredient.id}, Ingrediente: {ingredient.name}, Rendimento: {ingredient.rendimento}, Preço/Kg: {ingredient.price}"));

        internal bool Save( )
        {
            if (Error)
                return false;

            var processedData = LoadData();

            if (processedData.Any((Ingrediente) => Ingrediente.name == Name))
            {
                Console.WriteLine($"Ingrediente {Name} já existe");
                return false;
            }
            var id = processedData.OrderByDescending(ingrediente => ingrediente.id).FirstOrDefault().id;
            Write(processedData.Append(new IngredientStruct(id + 1, Name, Rendimento, Price)).Select(ingredient => ingredient.ToString()).ToArray());
            Console.WriteLine($"id: {id + 1}, Ingrediente: {Name}, Rendimento: {Rendimento}, Preço/Kg: {Price}");
            return true;
        }

        internal bool Update( )
        {
            if (Error)
                return false;

            var processedData = LoadData();

            if (!processedData.Any((Ingrediente) => Ingrediente.name == Name))
            {
                Console.WriteLine($"Ingrediente {Name} não existe");
                return false;
            }
            Write(processedData.Select(
                    ingredient => ingredient.name != Name
                    ? ingredient.ToString()
                    : new IngredientStruct(ingredient.id, Name, Rendimento, Price).ToString())
                .ToArray());
            var id = processedData.Where(Ingrediente => Ingrediente.name == Name).Single().id;
            Console.WriteLine($"id: {id}, Ingrediente: {Name}, Rendimento: {Rendimento}, Preço/Kg: {Price}");

            return true;
        }

        internal static bool Delete(string name)
        {
            var processedData = LoadData();
            if (!processedData.Any((Ingrediente) => Ingrediente.name == name))
            {
                Console.WriteLine($"Ingrediente {name} não existe");
                return false;
            }
            Write(processedData.Where(ingredient => ingredient.name != name).Select(ingrediente => ingrediente.ToString()).ToArray());
            return true;
        }

        public static Ingrediente Create(string[] args)
        {
            var name = args[0];
            var error = false;
            if (!float.TryParse(args[1].Replace('.', ','), out var rendimento))
            {
                Console.WriteLine($"Rendimento em fomato invalido {args[1]}");
                error = true;
            }
            if (!float.TryParse(args[2].Replace('.', ','), out var price))
            {
                Console.WriteLine($"Preço em fomato invalido {args[2]}");
                error = true;
            }
            return new Ingrediente(name, rendimento, price, error);
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