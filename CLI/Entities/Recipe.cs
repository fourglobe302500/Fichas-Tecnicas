﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using CLI.Entities.Structs;
using CLI.Utils;

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
                        recipe.id.ToString(), recipe.name, recipe.links.ToArrayString()});
            Data.PrettyPrint(new string[] { "id", "Recipe", "Links" });
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

        /* TODO

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

        */

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
                        current = (0, 0, 0);
                        continue;
                }
            }
            return result.ToArray();
        }

        #endregion Utilities
    }
}