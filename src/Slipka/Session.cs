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
            Id = Guid.NewGuid().ToString();
            Calls = new List<Call>();
            Tags = new List<string>();
            RecordedCalls = new List<CallTemplate>();
            InterceptedCalls = new List<CallTemplate>();
        }

        [BsonId]
        public ObjectId InternalId { get; set; }
        [BsonElement("public_id")]
        public string Id { get; set; }
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
        [BsonElement]
        public List<string> Tags { get; set; }
        [BsonElement("recorded_calls")]
        public List<CallTemplate> RecordedCalls { get; set; }
        [BsonElement("overridden_calls")]
        public List<CallTemplate> InterceptedCalls { get; set; }
    }
}
