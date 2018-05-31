using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
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

        [BsonId]
        public ObjectId InternalId { get; set; }

        [BsonElement("content")]
        public ObjectId Content { get; set; }

        [BsonElement("headers")]
        public List<Header> Headers { get; set; }
    }
}
