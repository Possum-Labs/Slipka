using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace PossumLabs.Specflow.Core.Variables
{
    public class Interpeter
    {
        public Interpeter()
        {
            Repositories = new List<IRepository>();
            HasIndexer = new Regex(@"\[([0-9]+)\]", RegexOptions.Compiled);
            MatchVariable = new Regex(@"^[a-zA-Z][0-9a-zA-Z]*((?:\[[0-9]+\])|(?:\.[a-zA-Z][0-9a-zA-Z]*))*$", RegexOptions.Compiled);
        }

        private List<IRepository> Repositories { get; set; }
        private const char Sepetator = '.';
        private Regex HasIndexer { get; }
        private Regex MatchVariable { get; }

        public void Register(IRepository repository) 
            => Repositories.Add(repository);

        public void Set<X>(string path, X value)
        {
            var target = Walker(path.Split(Sepetator), 1);
            var prop = target.GetType().GetProperty(path.Split(Sepetator).Last());
            prop.SetValue(target, value);
        }

        public object Resolve(string path) 
            => Walker(path.Split('.'));

        public object Get(Type t, string path) 
            => IsVarialbe(path) ? Convert(t, Resolve(path)) : Convert(t, path);

        public X Get<X>(string path) 
            => Convert<X>(Resolve(path));

        private Func<object, object> ResolveIndexFactory(string rawRoot, out string root, IEnumerable<string> path)
        {
            if (HasIndexer.IsMatch(rawRoot))
            {
                var index = int.Parse(HasIndexer.Matches(rawRoot)[0].Groups[1].Value);
                root = rawRoot.Substring(0, rawRoot.IndexOf('['));
                return (source) =>
                {
                    if (source == null)
                        throw new GherkinException($"Unable to resolve [{ index }] of { path.Aggregate((x, y) => x + '.' + y)}");

                    var indexers = source.GetType().CachedGetProperties().Where(p => p.Name == "Item" && p.GetIndexParameters().One());
                    if (!indexers.Any())
                        throw new GherkinException($"The type of {source.GetType()} does not support [{ index }] of { path.Aggregate((x, y) => x + '.' + y)}");

                    //TODO: v2 add exception handeling
                    return indexers.First().GetValue(source,index.AsObjectArray());
                };
            }
            else
            {
                root = rawRoot;
                return (source) => source;
            }
        }

        public void Add<T>(string name, T item) where T:IValueObject
            => Repositories.First(x => x.Type == typeof(T)).Add(name, item);

        private object Walker(IEnumerable<string> path, int leave = 0)
        {
            var parts = path.Skip(1);
            var rawRoot = path.First();
            var indexResolver = ResolveIndexFactory(rawRoot, out string root, path);

            var repo = Repositories.FirstOrDefault(x => x.ContainsKey(root));
            
            if (repo == null)
                return rawRoot;

            var current = indexResolver((object)repo[root]);

            while (parts.Count() > leave)
            {
                try
                {
                    indexResolver = ResolveIndexFactory(parts.First(), out string part, path);
                    var prop = current.GetType().GetProperty(part);
                    current = prop.GetValue(current);
                    current = indexResolver(current);
                    parts = parts.Skip(1);
                }
                catch (InvalidOperationException)
                {
                    throw new GherkinException($"Unable to resolve {parts.First()} of {path.LogFormat()}. " +
                        $"Did find {current.GetType().CachedGetProperties().Select(x => x.Name).LogFormat()}");
                }
            }
            return current;
        }

        private bool IsVarialbe(string path) => MatchVariable.IsMatch(path);

        public X Convert<X>(object o) => (X)Convert(typeof(X), o);

        public object Convert(Type t, object o)
        {
            if (o == null)
            {
                if (t.IsValueType)
                    return Activator.CreateInstance(t);
                return null;
            }

            var sourceType = o.GetType();

            if (t.IsAssignableFrom(sourceType))
                return o;

            var conversions = Repositories.Where(x => x.Type == t).SelectMany(x => x.RegisteredConversions).Where(c => c.Test.Invoke(o));
            if (conversions.Any())
                return conversions.First().Conversion.Invoke(o);

            //handle nullables
            var targetType = t;
            if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(Nullable<>))
                targetType = Nullable.GetUnderlyingType(targetType);

            var convertConversions = typeof(System.Convert).CachedGetMethods()
                .Where(x => x.ReturnType == targetType && x.Name.StartsWith("To") && x.GetParameters().Any(p => p.ParameterType == sourceType));

            if (convertConversions.Any())
                return convertConversions.First().Invoke(null, o.AsObjectArray());

            if (targetType.IsEnum && sourceType == typeof(string))
                return Enum.Parse(targetType, o as string);

            if (targetType.GetConstructors().Where(x => x.IsPublic && !x.IsStatic).Select(c => c.GetParameters())
                .Any(x => x.One() && x.First().ParameterType == sourceType))
                return Activator.CreateInstance(targetType, o);

            var parseMethod = targetType.CachedGetMethods().Where(m=>m.IsStatic && m.IsPublic)
                .Where(m => m.Name == "Parse" && m.GetParameters().Length == 1 && m.GetParameters().First().ParameterType == sourceType);

            if (parseMethod.Any())
                return parseMethod.First().Invoke(null, o.AsObjectArray());

            if (targetType == typeof(string))
                return o.ToString();

            if(sourceType == typeof(string) && ((string)o).IsValidJson())
                return JsonConvert.DeserializeObject((string)o, t);

            throw new GherkinException($"Unable to convert from {sourceType} to {t}");
        }
    }
}
