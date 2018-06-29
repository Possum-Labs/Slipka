using Slipka.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Slipka.ApiArguments
{
    public class DecorateMessage
    {
        public string Key { get; set; }
        public List<string> Values { get; set; }

        public static implicit operator Header(DecorateMessage m)
            => new Header(m.Key, m.Values);

        public Header AsHeader => this;
    }
}
