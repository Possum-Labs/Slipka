using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace PossumLabs.Specflow.Core
{
    public class RepositoryBase<T> : IRepository, IEnumerable<KeyValuePair<string,T>>
        where T : IValueObject
    {
        public RepositoryBase()
        {
            SetupDefaultConversions();
        }

        private Dictionary<string, IValueObject> dictionary = new Dictionary<string, IValueObject>();
        private List<TypeConverter> conversions = new List<TypeConverter>();

        public IValueObject this[string key] => dictionary[key];
        public void Add(string key, IValueObject item) => dictionary.Add(key, item);
        public Type Type => typeof(T);
        public IEnumerable<TypeConverter> RegisteredConversions => conversions;
        public bool ContainsKey(string root) => dictionary.ContainsKey(root);

        public void RegisterConversion<C>(Func<C, T> conversion, Predicate<C> test)=>
            conversions.Add(new TypeConverter((x) => conversion.Invoke((C)x), x => test.Invoke((C)x)));

        protected virtual void SetupDefaultConversions()
        {
            RegisterConversion<object>(
                c => (T)c, 
                c => typeof(T).IsAssignableFrom(c.GetType()));

            RegisterConversion<string>(
                c => JsonConvert.DeserializeObject<T>((string)c), 
                c => typeof(string).IsAssignableFrom(c.GetType()) && ((string)c).IsValidJson());
        }

        public IEnumerator<KeyValuePair<string,T>> GetEnumerator() =>
            dictionary.ToDictionary(x=>x.Key, x=>(T)x.Value).ToList().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
