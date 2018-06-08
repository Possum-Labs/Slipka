using Slipka.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Slipka.Configuration
{
    [ConfigurationObject("ProxySettings")]
    public class ProxySettings
    {
        [ConfigurationMember("FirstPort")]
        public int FirstPort { get; set; }
        [ConfigurationMember("LastPort")]
        public int LastPort { get; set; }
    }
}
