using System;
using System.Collections.Generic;
using System.Linq;

using CLI.Entities.Structs;

namespace CLI.Utilities
{
    internal static class Utils
    {
        public static string ToArrayString(this Link[] values)
        {
            if (values == null)
                return "[]";
            string res = "";
            values.Select(obj => obj.ToString()).ForEach(str => res += str + ", ");
            return res.Length >= 2 ? $"[{res[0..^2]}]" : "[]";
        }

        public static string ToArrayString(this object[] values)
        {
            if (values == null)
                return "[]";
            string res = "";
            values.Select(obj => obj.ToString()).ForEach(str => res += str + ", ");
            return res.Length >= 2 ? $"[{res[0..^2]}]" : "[]";
        }

        public static void WriteValues(string name = null, params (object value, string name)[] values)
        {
            var text = new string[(values.Length * 2) + (name == null ? 1 : 3)];
            int BiggestName = 0, BiggestValue = 0;
            text[0] = "┌";
            for (var i = 0; i < text.Length - 1; i += 2)
            {
                text[i + 1] = "│";
                text[i + 2] = "├";
            }
            text[^1] = "└";
            for (var i = 0; i < values.Length; i++)
            {
                BiggestName = Math.Max(BiggestName, values[i].name.Length);
                BiggestValue = Math.Max(BiggestValue, values[i].value.ToString().Length + 1);
            }
            text[name == null ? 0 : 2] += new string('─', BiggestName + BiggestValue + 1) + (name == null ? '┐' : '┤');
            var last = true;
            int index(int i) => (i - (name == null ? 1 : 3)) / 2;
            for (var i = text.Length - 1; i >= (name == null ? 1 : 3); i -= 2)
            {
                text[i - 1] += $"{values[index(i)].name}{new string(' ', BiggestName - values[index(i)].name.Length)}: " +
                    $"{values[index(i)].value}{new string(' ', BiggestValue - values[index(i)].value.ToString().Length - 1)}│";
                text[i] += new string('─', BiggestName + BiggestValue + 1) + (last ? '┘' : '┤');
                last = false;
            }
            if (name != null)
            {
                text[0] += new string('─', BiggestName + BiggestValue + 1) + '┐';
                text[1] += name + new string(' ', BiggestName + BiggestValue - name.Length + 1) + '│';
            }
            text.ForEach(line => Console.WriteLine(line));
        }

        public static void PrettyPrint(this IEnumerable<string> values, string[] keys, string name = null) => values.ToArray().PrettyPrint(keys, name, values.Count(), keys.Length);
        /* print logic
         *
         * header
         * line 1
         * line 2
         * line 3
         *
         *                              ┌ ┐ └ ┘ ├ ┤ ┬ ┴ ┼ ─ │
         * text 0                       ┌──┬─────────┬──────┐
         * header/text 1                │id│Nome     │Preço │
         * separator 0/text 2           ├──┼─────────┼──────┤
         * line 1/text 3                │0 │tomate   │1.5   │
         * separator 1/text 4           ├──┼─────────┼──────┤
         * line 2/text 5                │1 │macarrão │22.8  │
         * separator 2/text 6           ├──┼─────────┼──────┤
         * line 3/text 7                │2 │beterraba│560,96│
         * end or box text 8            └──┴─────────┴──────┘
         * */

        public static void PrettyPrint(this string[] values, string[] keys, string name, int valuesLength, int keysLenght)
        {
            int lastId = (((valuesLength / keysLenght) + 1) * 2) + (name == null ? 0 : 2);
            var text = new string[lastId + 1];
            int lines = valuesLength / keysLenght;
            text[0] = "┌";
            text[1] = "│";
            for (var i = 0; i < lines * 2 + (name == null ? 0 : 2); i += 2)
            {
                text[i + 2] = "├";
                text[i + 3] = "│";
            }
            text[lastId] = "└";
            var first = true;
            var totalSize = 0;
            for (var keyIndex = 0; keyIndex < keysLenght; keyIndex++)
            {
                int index(int i) => (i / 2 * keysLenght) + keyIndex;
                // default biggest to the key lenght
                int Biggest = keys[keyIndex].Length;
                //run thru line to see if have a bigger one
                for (var i = 0; i < lines * 2; i++)
                    Biggest = Math.Max(Biggest, values[index(i)].Length);
                totalSize += Biggest;
                //append to header line
                if (!first)
                    text[(name == null ? 0 : 2)] = text[(name == null ? 0 : 2)][0..^1] + '┬';
                text[(name == null ? 0 : 2)] += new string('─', Biggest) + (name == null ? '┐' : '┤');
                text[name == null ? 1 : 3] += $"{keys[keyIndex]}{new string(' ', Biggest - keys[keyIndex].Length)}│";
                //append to all otter lines
                for (var i = 0; i < lines * 2; i += 2)
                {
                    text[i + (name == null ? 3 : 5)] += $"{values[index(i)]}{new string(' ', Biggest - values[index(i)].Length)}│";
                    if (!first)
                        text[i + (name == null ? 2 : 4)] = text[i + (name == null ? 2 : 4)][0..^1] + '┼';
                    text[i + (name == null ? 2 : 4)] += new string('─', Biggest) + '┤';
                }
                if (!first)
                    text[lastId] = text[lastId][0..^1] + '┴';
                text[lastId] += new string('─', Biggest) + '┘';
                first = false;
            }
            if (name != null)
            {
                text[0] += new string('─', totalSize + keysLenght - 1) + '┐';
                text[1] += name + new string(' ', totalSize + keysLenght - name.Length - 1) + '│';
            }
            text.ForEach(line => Console.WriteLine($"{line}"));
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