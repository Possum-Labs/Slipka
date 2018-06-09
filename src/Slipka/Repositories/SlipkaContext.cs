using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using Slipka.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Slipka
{
    public class SlipkaContext
    {
        private readonly IMongoDatabase _database;

        public SlipkaContext(MongoSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            _database = client.GetDatabase(settings.Database);

            Bucket = new GridFSBucket(_database, options: new GridFSBucketOptions
            {
                BucketName = "bodies",
                ChunkSizeBytes = 1048576, // 1MB
            });
        }

        public IMongoCollection<Session> Sessions
        {
            get
            {
                return _database.GetCollection<Session>("Session");
            }
        }

        public IMongoCollection<Message> Messages
        {
            get
            {
                return _database.GetCollection<Message>("Message");
            }
        }

        public IGridFSBucket Bucket { get; }
    }
}
