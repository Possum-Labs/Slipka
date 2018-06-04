﻿using MongoDB.Bson;
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
            TaggedCalls = new List<CallTemplate>();
            Decorations = new List<Header>();
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
        [BsonElement("tagged_calls")]
        public List<CallTemplate> TaggedCalls { get; set; }
        [BsonElement("decorations")]
        public List<Header> Decorations { get; set; }

        [BsonIgnore]
        public bool Active { get; set; }

        public int State()
        {
            int state = 0;
            lock(Calls)
            {
                state += Calls.Count(x=>x.RequestId != ObjectId.Empty);
                state += Calls.Count(x => x.ResponseId != ObjectId.Empty);
                state += Tags.Count();
            }
            return state;
        }
    }
}