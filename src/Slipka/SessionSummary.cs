using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Slipka
{
    public class SessionSummary
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int? CallsCount { get; set; }
        public int ProxyPort { get; set; }
        public string TargetHost { get; set; }
        public int TargetPort { get; set; }
        public List<string> Tags { get; set; }
        public List<Call> RecordedCalls { get; set; }
        public List<Call> OverriddenCalls { get; set; }
    }
}
