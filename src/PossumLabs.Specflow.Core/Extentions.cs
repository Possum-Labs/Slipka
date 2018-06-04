using System;
using System.Collections;
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

        public static bool None<T>(this IEnumerable<T> l)
            => !l.Any();

        public static Exception[] GetFailedValidations(this object o, IEnumerable<Validation> validations)
            => validations
                .Select(x => x.Validate(o))
                .Where(x => x != null)
                .ToArray();

        public static void Validate(this object o, params Validation[] validations)
            => o.Validate(validations);

        public static void Validate(this object o, IEnumerable<Validation> validations)
        {
            var failedVaidations = o.GetFailedValidations(validations);

            if (failedVaidations.Any())
                throw new AggregateException(failedVaidations.OrderBy(e => e.Message.Length));
        }

        public static bool Contains(this IEnumerable o, Validation validation)
            => o.Cast<object>().Where(x => validation.Validate(x) == null).Any();

        public static bool Contains(this IEnumerable o, IEnumerable<Validation> validations)
            => o.Cast<object>().Where(x => x.GetFailedValidations(validations).None()).Any();

        public static bool Contains(this IEnumerable o, IEnumerable<IEnumerable<Validation>> validationRows)
            => validationRows.Where(validations => !o.Cast<object>().Contains(validations)).None();

        public static bool Contains(this object o, IEnumerable<IEnumerable<Validation>> validationRows)
            => o.ConvertToIEnumerable().Contains(validationRows);

        public static bool Contains(this object o, Validation validation)
            => o.ConvertToIEnumerable().Contains(validation);

        public static IEnumerable ConvertToIEnumerable(this object o)
        {
            if (o == null)
                throw new Exception($"The referenced variable does not have a value.");
            if (o is IEnumerable)
                return ((IEnumerable)o);
            throw new GherkinException("The referenced variable is not enumerable.");
        }

    }
}
