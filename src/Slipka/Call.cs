using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Slipka
{
    public class Call
    {
        public Call()
        {
            Recieved = DateTime.UtcNow;
        }

        [BsonElement("response")]
        public Message Response { get; set; }
        [BsonElement("request")]
        public Message Request { get; set; }
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
        [BsonElement("overridden")]
        public bool Intercepted { get; set; }

        [BsonElement("overridden")]
        public bool Recorded { get; set; }

        [BsonElement("recieved")]
        public DateTime Recieved { get; set; }
    }
}
