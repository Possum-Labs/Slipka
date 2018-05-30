using PossumLabs.Specflow.Core;
using System;
using System.Collections.Generic;

namespace PossumLabs.Specflow.Slipka
{
    public class Call : IDomainObject
    {
        public Call()
        {

        }

        public Call(string Host, string Path)
        {
            Uri = new Uri($"{Host}/{Path}");
        }

        public Message Response { get; set; }
        public Message Request { get; set; }
        public string StatusCode { get; set; }

        public string Method { get; set; }
        public Uri Uri { get; set; }

        public double? Duration { get; set; }

        public bool Overridden { get; set; }
        public bool Recorded { get; set; }

        public List<string> Tags { get; set; }

        public DateTime Recieved { get; set; }
    }
}
