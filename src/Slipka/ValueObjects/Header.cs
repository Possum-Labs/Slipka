using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Slipka.ValueObjects
{
    public class Header
    {
        public Header()
        {
            Values = new List<string>();
        }

        public Header(string key, IEnumerable<string> value)
        {
            Key = key;
            Values = value== null?new List<string>() : value.ToList();
        }

        public string Key { get; set; }
        public List<string> Values { get; set; }
    }
}
