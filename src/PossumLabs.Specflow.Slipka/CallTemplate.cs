using PossumLabs.Specflow.Core;
using System;

namespace PossumLabs.Specflow.Slipka
{
    public class CallTemplate : IDomainObject
    {
        public CallTemplate()
        {

        }

        public Message Response { get; set; }
        public Message Request { get; set; }
        public string StatusCode { get; set; }

        //request
        public string Method { get; set; }
        public string Uri { get; set; }

        //metadata
        public int? Duration { get; set; }
    }
}
