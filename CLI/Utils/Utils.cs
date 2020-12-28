using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLI.Utils
{
    internal static class Utils
    {
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