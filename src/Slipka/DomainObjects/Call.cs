using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Slipka.DomainObjects
{
    public class Call
    {
        public Call()
        {
            Recieved = DateTime.UtcNow;
            Tags = new List<string>();
        }

        [BsonElement("response")]
        public ObjectId ResponseId { get; set; }
        [BsonElement("request")]
        public ObjectId RequestId { get; set; }
        [BsonElement("status_code")]
        public int? StatusCode { get; set; }
 
        //request
        [BsonElement("method")]
        public string Method { get; set; }
        [BsonElement("uri")]
        public Uri Uri { get; set; }

        //metadata
        [BsonElement("duration")]
        public double? Duration { get; set; }
        
        //override
        [BsonElement("intercepted")]
        public bool Injected { get; set; }

        [BsonElement("recorded")]
        public bool Recorded { get; set; }

        [BsonElement("recieved")]
        public DateTime Recieved { get; set; }

        [BsonElement]
        public List<string> Tags { get; set; }
    }
}
