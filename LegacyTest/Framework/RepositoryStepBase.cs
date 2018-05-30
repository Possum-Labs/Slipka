using PossumLabs.Specflow.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace LegacyTest
{
    public abstract class RepositoryStepBase<T> : StepBase
        where T : IValueObject
    {
        public RepositoryStepBase(ScenarioContext scenarioContext, FeatureContext featureContext) : base(scenarioContext, featureContext)
        {
            Repository = new RepositoryBase<T>();
        }

        private RepositoryBase<T> Repository;

        protected T this[string name]
        {
            get => (T)Repository[name];
        }

        protected void Add(string name, T item)
        {
            if (Repository.ContainsKey(name))
                throw new GherkinException($"There is already a varaible of type {typeof(T)} and name {name}");
            Repository.Add(name, item);
        }

        abstract protected void Create(T item);

        [BeforeScenario]
        public void RegisterRepositoryWithInterpeter() => Interpeter.Register(Repository);

        [StepArgumentTransformation]
        public List<T> TransformList(Table table)
        {
            var dupes = table.Header.GroupBy(x => splitter(x).Aggregate((y, z) => y + "." + z).ToUpper()).Where(x => x.Count() > 1);
            if (dupes.Any())
                throw new GherkinException($"the columns {dupes.LogFormat()} are effectively duplicates, matching of columns is case insesnitive");

            return table.Rows.Select(
                r => (T)Map(table.Header.ToDictionary(
                           x => x.ToUpper(),
                           x => new KeyValuePair<string, string>(x, r[x])
                       ), typeof(T))).ToList();
        }

        [StepArgumentTransformation]
        public Dictionary<string, T> TransformDictionary(Table table)
        {
            var dupes = table.Header.GroupBy(x => splitter(x).Aggregate((y,z)=>y+"."+z).ToUpper()).Where(x => x.Count() > 1);
            if (dupes.Any())
                throw new GherkinException($"the columns {dupes.LogFormat()} are effectively duplicates, matching of columns is case insesnitive");

            if(!table.Header.Contains(ParserRules.VaraibleKey))
                throw new GherkinException($"a column called \"{ParserRules.VaraibleKey}\" is required for this step");

            return table.Rows.ToDictionary(
                r => r[ParserRules.VaraibleKey],
                r => (T)Map(table.Header.Except(new[] { ParserRules.VaraibleKey }).ToDictionary(
                           x => x.ToUpper(),
                           x => new KeyValuePair<string, string>(x, r[x])
                       ), typeof(T)));
        }

        //TODO: refactor to utlility class
        IEnumerable<string> splitter(string name) => name.Split(new[] { '.', ' ' });

        string recurse(string name) => splitter(name).Skip(1).Aggregate((x, y) => x + " " + y);

        private object Map(Dictionary<string, KeyValuePair<string,string>> values, Type desiredType)
        {
            var properties = desiredType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty);
            var constructors = desiredType.GetConstructors(BindingFlags.Public | BindingFlags.Instance);

            var groups = values.Keys.Select(x => splitter(x).First());
            var unmatched = groups.Except(properties.Select(x => x.Name.ToUpper()));
            var errors = unmatched.Except(constructors.SelectMany(c => c.GetParameters().Select(p => p.Name.ToUpper())));

            if (errors.Any()) {
                var prefix = values[errors.First()].Key.Substring(0, values[errors.First()].Key.Length - errors.First().Length);
                var unused = properties.Select(p => p.Name).Where(p => !groups.Contains(p.ToUpper()));
                throw new GherkinException($"The columns:{errors.LogFormat(x => values[x].Key)} are unmatched maybe it is one of these {unused.LogFormat(x=>prefix+x)}");
            }

            //TODO: refactor to utlility class
            object resolve(string name, Type t)
            {
                var keys = values.Keys.Where(k => splitter(k).First() == name);
                object item;
                if (keys.Count() == 1 && splitter(keys.First()).Count() == 1)
                    item = Interpeter.Get(t, values[name].Value);
                else if (keys.Any(k => splitter(k).Count() == 1))
                    throw new GherkinException($"You can't specify the object and set properties on the object, columns {keys.LogFormat(x=>values[x].Key)} are in conflict");
                else
                    item = Map(keys.ToDictionary(x => recurse(x), x => values[x]), t);

                foreach (var k in keys.ToList())
                    values.Remove(k);

                return item;
            };

            object ret;
            if (unmatched.Any())
            {
                var possibles = constructors.Where(c => unmatched.Except(c.GetParameters().Select(p => p.Name.ToUpper())).Count() == 0);
                var valid = possibles.Where(c => c.GetParameters().Select(p => p.Name.ToUpper()).Except(groups).Count() == 0);
                var ctor = valid.OrderBy(c => c.GetParameters().Count()).Reverse().First();

                ret = ctor.Invoke(ctor.GetParameters().Select(p => resolve(p.Name.ToUpper(), p.ParameterType)).ToArray());
            }
            else
                ret = Activator.CreateInstance(desiredType);

            foreach (var key in values.Keys.Select(k => splitter(k).First()).ToList())
            {
                var prop = properties.First(p => p.Name.ToUpper() == key);
                prop.SetValue(ret, resolve(key, prop.PropertyType));
            }

            return ret;
        }

        [StepArgumentTransformation]
        public T Transform(string id) => Interpeter.Get<T>(id);

        [AfterScenario]
        public void Records()
        {
            if (Repository.Any())
            {
                Trace.WriteLine($"Repository for {typeof(T).Name}");
                foreach (var item in Repository.Where(x => x.Value != null))
                    Trace.WriteLine($"Key:{item.Key} Id:{item.Value}");
            }
        }
    }
}
