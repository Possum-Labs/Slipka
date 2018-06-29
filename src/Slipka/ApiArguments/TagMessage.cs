using Slipka.ValueObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Slipka.ApiArguments
{
    public class TagMessage
    {
        public MessageTemplate Response { get; set; }

        public MessageTemplate Request { get; set; }

        public string StatusCode { get; set; }

        public string Method { get; set; }

        public string Uri { get; set; }

        public int? Duration { get; set; }

        [Required]
        public List<string> Tags { get; set; }

        public static implicit operator CallTemplate(TagMessage m)
            => new CallTemplate {
                Request = m.Request,
                Response = m.Response,
                Duration = m.Duration,
                Method = m.Method,
                StatusCode = m.StatusCode,
                Tags = m.Tags,
                Uri = m.Uri  };

        public CallTemplate AsCallTemplate => this;
    }
}
