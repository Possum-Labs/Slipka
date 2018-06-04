using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace PossumLabs.Specflow.Core
{
    public static class Json
    {
        public static bool IsValidJson(this string strInput)
        {
            strInput = strInput.Trim();
            if ((strInput.StartsWith("{") && strInput.EndsWith("}")) || //For object
                (strInput.StartsWith("[") && strInput.EndsWith("]"))) //For array
            {
                try
                {
                    var obj = JToken.Parse(strInput);
                    return true;
                }
                catch (JsonReaderException jex)
                {
                    //Exception in parsing json
                    Console.WriteLine(jex.Message);
                    return false;
                }
                catch (Exception ex) //some other exception
                {
                    Console.WriteLine(ex.ToString());
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public static IEnumerable<ValueMemberInfo> GetValueMembers(this Type t)
            => t.GetFields().Select(f => new ValueMemberInfo(f)).Concat(t.GetProperties().Select(p => new ValueMemberInfo(p)));

        public static bool HasAllPropertiesAndFieldsOf(this object target, object subset)
        {
            if (target == null && subset == null)
                return true;

            if (target == null || subset == null)
                return false;

            var targetType = target.GetType();
            var subsetType = subset.GetType();

            if (!targetType.IsInstanceOfType(subset))
                return false;


            if (target is IComparable && subset is IComparable)
                return 0 == ((IComparable)target).CompareTo(subset);
            
            if (target is IEnumerable  && subset is IEnumerable)
            {
                var targetValues = ((IEnumerable)target).AsQueryable().Cast<object>();
                var subsetValues = ((IEnumerable)subset).AsQueryable().Cast<object>();
                var missing = subsetValues.Except(targetValues, new EqualityComparer<object>((s, t) => t.HasAllPropertiesAndFieldsOf(s)));
                return missing.None();
            }

            var targetMembers = targetType.GetValueMembers();
            var subsetMembers = subsetType.GetValueMembers();
            foreach (var member in subsetMembers.Where(m => m.HasValue(subset)))
            {
                var targetMember = targetMembers.FirstOrDefault(m => m.Name == member.Name);
                if (targetMember == null || !targetMember.GetValue(target).HasAllPropertiesAndFieldsOf(member.GetValue(subset)))
                    return false;
            }

            return true;
        }

        public static object FromJsonAsTypeOf(this string source, object template)
        {
            return JsonConvert.DeserializeObject(source, template.GetType());
        }
    }
}
