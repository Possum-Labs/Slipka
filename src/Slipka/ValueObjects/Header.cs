using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Slipka
{
    public class Header
    {
        public Header(string key, IEnumerable<string> value)
        {
            Key = key;
            Values = value.ToList();
        }

        public string Key { get; set; }
        public List<string> Values { get; set; }
    }
}
