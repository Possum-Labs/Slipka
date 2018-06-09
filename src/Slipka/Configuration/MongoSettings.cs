using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Slipka.Configuration
{
    [ConfigurationObject("SLIPKA")]
    public class MongoSettings
    {
        [ConfigurationMember("ConnectionString")]
        public string ConnectionString { get; set; }
        [ConfigurationMember("Database")]
        public string Database { get; set; }
    }
}
