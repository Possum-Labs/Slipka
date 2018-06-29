using System;
using System.Collections.Generic;
using System.Text;

namespace PossumLabs.Specflow.Core.Variables
{
    public interface IRepository
    {
        bool ContainsKey(string root);
        Type Type { get; }
        IValueObject this[string key]
        { 
            get;
        }
        IEnumerable<TypeConverter> RegisteredConversions { get; }

        void Add(string key, IValueObject item);
    }
}
