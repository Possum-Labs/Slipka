using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Slipka.Configuration
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ConfigurationObjectAttribute : Attribute
    {
        public ConfigurationObjectAttribute(string path)
        {
            Path = path;
        }

        public string Path { get; set; }
    }
}
