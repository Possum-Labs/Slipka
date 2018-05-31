using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Slipka
{
    public class SlipkaContext
    {
        private readonly IMongoDatabase _database;

        public SlipkaContext(IOptions<MongoSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            _database = client.GetDatabase(settings.Value.Database);

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
