using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Slipka
{
    public class Session
    {
        public Session()
        {
            PublicId = Guid.NewGuid();
        }

        [BsonId]
        public ObjectId Id { get; set; }
        [BsonElement("public_id")]
        public Guid PublicId { get; set; }
        [BsonElement("name")]
        public string Name { get; set; }
        [BsonElement("calls")]
        public List<Call> Calls { get; set; }
        [BsonElement("proxy_port")]
        public int ProxyPort { get; set; }
        [BsonElement("target_host")]
        public string TargetHost { get; set; }
        [BsonElement("target_port")]
        public int TargetPort { get; set; }
    }
}
