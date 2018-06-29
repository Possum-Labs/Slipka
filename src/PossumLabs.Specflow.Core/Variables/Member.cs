using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace PossumLabs.Specflow.Core.Variables
{
    public class ValueMemberInfo
    {
        public ValueMemberInfo(PropertyInfo property)
            => Property = property;

        public ValueMemberInfo(FieldInfo field)
            => Field = field;

        PropertyInfo Property { get; }
        FieldInfo Field { get; }

        public string Name => (Property != null) ? Property.Name : Field.Name;
        public Type Type => (Property != null) ? Property.PropertyType : Field.FieldType;

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
            if (Property != null)
                Property.SetValue(source, value);
            else
                Field.SetValue(source, value);
        }
    }
}
