using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using Slipka.Configuration;
using Slipka.DomainObjects;
using Slipka.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Slipka.Repositories
{
    public class SlipkaContext
    {
        private IMongoDatabase Database { get; }

        public SlipkaContext(MongoSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            Database = client.GetDatabase(settings.Database);

            Bucket = new GridFSBucket(Database, options: new GridFSBucketOptions
            {
                BucketName = "bodies",
                ChunkSizeBytes = 1048576, // 1MB
            });

            CreateIndex();
        }

        async Task CreateIndex()
        {
            await Sessions.Indexes.DropAllAsync();
            await Sessions.Indexes.CreateOneAsync(Builders<Session>.IndexKeys.Ascending(_ => _.Id));
            await Sessions.Indexes.CreateOneAsync(Builders<Session>.IndexKeys.Text(_ => _.Tags));
            var command = @"
{
    createIndexes: ""Session"",
    indexes: [
    {
        key: {
            retain_data_until: 1
            },
        name: ""cleanupSessions"",
        expireAfterSeconds: 0, 
    }]
}";
            try
            {
                var res = await Database.RunCommandAsync<BsonDocument>(command);
            }
            catch(Exception ex)
            {
                //TODO: error logs?
            }

            await Messages.Indexes.DropAllAsync();
            command = @"
{
    createIndexes: ""Message"",
    indexes: [
    {
        key: {
            retain_data_until: 1
            },
        name: ""cleanupSessions"",
        expireAfterSeconds: 0, 
    }]
}";
            try
            {
                var res = await Database.RunCommandAsync<BsonDocument>(BsonDocument.Parse(command));
            }
            catch (Exception ex)
            {
                //TODO: error logs?
            }
        }

        public IMongoCollection<Session> Sessions => Database.GetCollection<Session>("Session");

        public IMongoCollection<Message> Messages => Database.GetCollection<Message>("Message");

        public IGridFSBucket Bucket { get; }
    }
}
