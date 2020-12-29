using System;
using System.Collections.Generic;
using System.Linq;

namespace CLI.Utils
{
    internal static class Utils
    {
        public static void PrettyPrint(this IEnumerable<string> values, string[] keys) => values.PrettyPrint(keys, keys.Length);

        public static void PrettyPrint(this IEnumerable<string> values, string[] keys, int keysLenght)
        {
            var text = new string[(values.Count() / keysLenght) + 1];
            for (var j = 0; j < keysLenght; j++)
            {
                int Biggest = keys[j].Length;
                for (var i = 0; i < values.Count() / keysLenght; i++)
                    Biggest = Math.Max(Biggest, values.ElementAt((i * keysLenght) + j).Length);
                text[0] = $"{text[0]}{keys[j]}{new string(' ', Biggest - keys[j].Length + 1)}|";
                for (var i = 0; i < values.Count() / keysLenght; i++)
                    text[i + 1] = $"{text[i + 1]}{values.ElementAt((i * keysLenght) + j)}{new string(' ', Biggest - values.ElementAt((i * keysLenght) + j).Length + 1)}|";
            }
            text.ForEach(line => Console.WriteLine($"|{line}"));
        }

        public static void ForEach<T>(this IEnumerable<T> vals, Action<T, int> func)
        {
            for (var i = 0; i < vals.Count(); i++)
            {
                func(vals.ElementAt(i), i);
            }
        }

        public static void ForEach<T>(this IEnumerable<T> vals, Action<T> func)
        {
            foreach (var val in vals)
            {
                func(val);
            }
        }
    }
}