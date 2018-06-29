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

        [ConfigurationMember("DefaultOpenFor")]
        public TimeSpan DefaultOpenFor { get; set; }
        [ConfigurationMember("MaxOpenFor")]
        public TimeSpan MaxOpenFor { get; set; }

        [ConfigurationMember("DefaultRetainedFor")]
        public TimeSpan DefaultRetainedFor { get; set; }
        [ConfigurationMember("MaxRetainedFor")]
        public TimeSpan MaxRetainedFor { get; set; }
    }
}
