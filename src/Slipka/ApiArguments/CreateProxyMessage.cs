using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Slipka.ApiArguments
{
    public class CreateProxyMessage
    {
        public CreateProxyMessage()
        {
            Tags = new List<string>();
            RecordedCalls = new List<RecordMessage>();
            InjectedCalls = new List<InjectMessage>();
            TaggedCalls = new List<TagMessage>();
            Decorations = new List<DecorateMessage>();
        }

        public string Name { get; set; }

        [Required]
        public string TargetHost { get; set; }
        public int? TargetPort { get; set; }

        public List<string> Tags { get; set; }

        public List<RecordMessage> RecordedCalls { get; set; }

        public List<InjectMessage> InjectedCalls { get; set; }

        public List<TagMessage> TaggedCalls { get; set; }

        public List<DecorateMessage> Decorations { get; set; }

        [IsFuture()]
        public string RetainedFor { get; set; }

        [IsFuture()]
        public string OpenFor { get; set; }
    }
}
