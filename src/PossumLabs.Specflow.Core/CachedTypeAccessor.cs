using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace PossumLabs.Specflow.Core
{
    public static class CachedTypeAccessor
    {
        static CachedTypeAccessor()
        {
            Properties = new Dictionary<Type, PropertyInfo[]>();
            Fields = new Dictionary<Type, FieldInfo[]>();
            Methods = new Dictionary<Type, MethodInfo[]>();
            Constructors = new Dictionary<Type, ConstructorInfo[]>();
        }

        private static Dictionary<Type, PropertyInfo[]> Properties { get; }

        public static PropertyInfo[] CachedGetProperties(this Type t)
        {
            if (Properties.ContainsKey(t))
                return Properties[t];
            lock(Properties)
            {
                if (!Properties.ContainsKey(t))
                    Properties.Add(t,t.GetProperties(BindingFlags.Public | BindingFlags.Instance));
            }
            return Properties[t];
        }

        private static Dictionary<Type, FieldInfo[]> Fields { get; }

        public static FieldInfo[] CachedGetFields(this Type t)
        {
            if (Fields.ContainsKey(t))
                return Fields[t];
            lock (Fields)
            {
                if (!Fields.ContainsKey(t))
                    Fields.Add(t, t.GetFields(BindingFlags.Public | BindingFlags.Instance));
            }
            return Fields[t];
        }

        private static Dictionary<Type, MethodInfo[]> Methods { get; }

        public static MethodInfo[] CachedGetMethods(this Type t)
        {
            if (Methods.ContainsKey(t))
                return Methods[t];
            lock (Methods)
            {
                if (!Methods.ContainsKey(t))
                    Methods.Add(t, t.GetMethods());
            }
            return Methods[t];
        }

        private static Dictionary<Type, ConstructorInfo[]> Constructors { get; }

        public static ConstructorInfo[] CachedGetConstructors(this Type t)
        {
            if (Constructors.ContainsKey(t))
                return Constructors[t];
            lock (Constructors)
            {
                if (!Constructors.ContainsKey(t))
                    Constructors.Add(t, t.GetConstructors(BindingFlags.Public | BindingFlags.Instance));
            }
            return Constructors[t];
        }
        
    }
}
