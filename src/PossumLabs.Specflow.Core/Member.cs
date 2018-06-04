using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace PossumLabs.Specflow.Core
{
    public class ValueMemberInfo
    {
        public ValueMemberInfo(PropertyInfo property)
            => Property = property;

        public ValueMemberInfo(FieldInfo field)
            => Field = field;

        PropertyInfo Property { get; }
        FieldInfo Field { get; }

        public string Name => Property.Name ?? Field.Name;

        public bool HasValue(object source)
        {
            var value = GetValue(source);
            if (value == null)
                return false;
            if (value.Equals(Activator.CreateInstance(value.GetType())))
                return false;
            return true;
        }

        public object GetValue(object source)
        {
            if (Property != null)
                return Property.GetValue(source);
            return Field.GetValue(source);
        }

        public void SetValue(object source, object value)
        {

        }
        
    }
}
