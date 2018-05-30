using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Slipka
{
    public class MessageTemplate
    {
        public MessageTemplate()
        {
            Headers = new List<KeyValuePair<string, List<string>>>();
        }
        [BsonElement("content")]
        public string Content { get; set; }
        [BsonElement("headers")]
        public List<KeyValuePair<string, List<string>>> Headers { get; set; }
    }
}
