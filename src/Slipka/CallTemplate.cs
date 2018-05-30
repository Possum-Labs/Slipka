using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Slipka
{
    public class CallTemplate
    {
        public CallTemplate()
        {
        }

        [BsonElement("response")]
        public MessageTemplate Response { get; set; }
        [BsonElement("request")]
        public MessageTemplate Request { get; set; }
        [BsonElement("status_code")]
        public string StatusCode { get; set; }
 
        //request
        [BsonElement("method")]
        public string Method { get; set; }
        [BsonElement("uri")]
        public string Uri { get; set; }

        //metadata
        [BsonElement("duration")]
        public int? Duration { get; set; }
        
        //override
        [BsonElement("overridden")]
        public bool Intercepted { get; set; }
    }
}
