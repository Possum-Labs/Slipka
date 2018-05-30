using System;
using System.Collections.Generic;
using System.Text;

namespace PossumLabs.Specflow.Core
{
    public class TypeConverter
    {
        public TypeConverter(Func<object, IValueObject> conversion, Predicate<object> test)
        {
            Conversion = conversion;
            Test = test;
        }
        public Func<object, IValueObject> Conversion { get; private set; }
        public Predicate<object> Test { get; private set; }
    }
}
