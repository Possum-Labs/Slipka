using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Slipka.ValueObjects
{
    public class MessageTemplate
    {
        public MessageTemplate()
        {
            Headers = new List<Header>();
        }
        [BsonElement("content")]
        public string Content { get; set; }
        [BsonElement("headers")]
        public List<Header> Headers { get; set; }
    }
}
