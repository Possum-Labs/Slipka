using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Slipka
{
    public class ProxySettings
    {
        //HACK: remove this
        public ProxySettings()
        {
            FirstPort = 7000;
            LastPort = 8000;
        }
        public int FirstPort { get; set; }
        public int LastPort { get; set; }
    }
}
