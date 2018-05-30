using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PossumLabs.Specflow.Core
{
    public static class Extentions
    {
        public static string LogFormat(this IEnumerable<string> l)
            => l.OrderBy(x=>x.ToLower()).Aggregate((x, y) => x + ", " + y);

        public static string LogFormat(this IEnumerable<IEnumerable<string>> l)
            => l.Select(x=>x.LogFormat()).Aggregate((x, y) => x + "\n" + y);

        public static string LogFormat<T>(this IEnumerable<T> l, Func<T, string> f)
            => l.Select(x => f.Invoke(x)).LogFormat();

        public static string LogFormat<T>(this IEnumerable<IEnumerable<T>> l, Func<T, string> f)
            => l.Select(x=>x.Select(y=>f.Invoke(y))).LogFormat();

        public static T ToEnum<T>(this string name) where T : struct
        {
            T e;
            if (!Enum.TryParse<T>(name, out e))
                throw new GherkinException($"Unable to conver {name} to Enumeration {typeof(T).Name} please use one of these {Enum.GetNames(typeof(T)).LogFormat()}");
            return e;
        }
    }
}
