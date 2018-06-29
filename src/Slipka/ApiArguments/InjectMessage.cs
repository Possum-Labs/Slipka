using Slipka.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Slipka.ApiArguments
{
    public class InjectMessage
    {
        public MessageTemplate Response { get; set; }
  
        public MessageTemplate Request { get; set; }
 
        public string StatusCode { get; set; }

        public string Method { get; set; }
  
        public string Uri { get; set; }

        public int? Duration { get; set; }

        public static implicit operator CallTemplate(InjectMessage m)
            => new CallTemplate
            {
                Request = m.Request,
                Response = m.Response,
                Duration = m.Duration,
                Method = m.Method,
                StatusCode = m.StatusCode,
                Uri = m.Uri
            };
        public CallTemplate AsCallTemplate => this;
    }
}
