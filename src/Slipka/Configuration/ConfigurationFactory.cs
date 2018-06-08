using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Slipka.Configuration
{
    public class ConfigurationFactory
    {
        public ConfigurationFactory(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public T Create<T>()
        {
            T ret = Activator.CreateInstance<T>();

            var type = typeof(T);

            string environmentVarName = null;

            var typeAttributes = type.GetCustomAttributes(typeof(ConfigurationObjectAttribute), false);
            if (typeAttributes.Any())
                environmentVarName = ((ConfigurationObjectAttribute)typeAttributes.First()).Path;

            var properties = type.GetProperties();
            foreach(var property in properties)
            {
                string environmentPropName = null;
                var propAttributes = property.GetCustomAttributes(typeof(ConfigurationMemberAttribute), false);
                if (propAttributes.Any())
                    environmentPropName = ((ConfigurationMemberAttribute)typeAttributes.First()).Path;

                string valueOverride = null;
                try
                {
                    valueOverride = Configuration.GetSection($"{type.Name}:{property.Name}").Value;
                }
                catch
                { }
                try
                {
                    if((!string.IsNullOrWhiteSpace(environmentVarName)) && (!String.IsNullOrWhiteSpace(environmentPropName)))
                        valueOverride = Environment.GetEnvironmentVariable($"{environmentVarName}_{environmentPropName}");
                }
                catch
                { }

                if (String.IsNullOrWhiteSpace(valueOverride))
                    continue;

                if (property.PropertyType == typeof(string))
                    property.SetValue(ret, valueOverride);

                if (property.PropertyType == typeof(int))
                    property.SetValue(ret, Convert.ToInt32(valueOverride));

                if (property.PropertyType == typeof(bool))
                    property.SetValue(ret, Convert.ToBoolean(valueOverride));

                throw new NotImplementedException($"Property of Type {property.PropertyType} is not supported.");
            }

            if(type.GetFields().Any())
                throw new NotImplementedException($"Fields are not supported for ConfigurationFactory");

            return ret;


        }
    }
}
