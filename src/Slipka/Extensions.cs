using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Slipka
{
    public static class Extensions
    {
        public static bool None<T>(this IEnumerable<T> l)
            => !l.Any();

        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> l, Action<T> a)
        {
            foreach (var i in l)
                a(i);
            return l;
        }
    }
}
