using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Slipka
{
    public class Message
    {
        public Message()
        {
            Headers = new List<Header>();
        }

        public static Message Empty = new Message();

        [JsonIgnore]
        [BsonId]
        public ObjectId InternalId { get; set; }

        [BsonElement("content")]
        public ObjectId ContentId { get; set; }

        [BsonElement("headers")]
        public List<Header> Headers { get; set; }
    }
}
