using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Slipka
{
    public class ProxyStore
    {
        public ProxyStore()
        {
            Proxies = new List<Proxy>();
        }
        public List<Proxy> Proxies { get; set; }
    }
}
