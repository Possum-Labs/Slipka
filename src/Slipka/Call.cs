using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Slipka
{
    public class Call
    {
        //response
        [BsonElement("response_content")]
        public byte[] ResponseContent { get; set; }
        [BsonElement("response_headers")]
        public List<KeyValuePair<string, List<string>>> ResponseHeaders { get; set; }
        [BsonElement("is_success_status_code")]
        public bool IsSuccessStatusCode { get; set; }
        [BsonElement("reason_phrase")]
        public string ReasonPhrase { get; set; }
        [BsonElement("status_code")]
        public string StatusCode { get; set; }

        //request
        [BsonElement("request_content")]
        public byte[] RequestContent { get; set; }
        [BsonElement("request_headers")]
        public List<KeyValuePair<string, List<string>>> RequestHeaders { get; set; }
        [BsonElement("method")]
        public string Method { get; set; }
        [BsonElement("request_uri")]
        public Uri RequestUri { get; set; }

        //metadata
        [BsonElement("duration")]
        public double Duration { get; set; }
        [BsonElement("content_size")]
        public long ContentSize { get; set; }

        //override
        [BsonElement("overridden")]
        public bool Overridden { get; set; }
    }
}
