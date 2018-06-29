using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using Slipka.Configuration;
using Slipka.DomainObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Slipka.Repositories
{
    public class FileRepository : IFileRepository
    {

        private SlipkaContext Context { get; }

        public FileRepository(SlipkaContext context)
        {
            Context = context;
        }

        public void Delete(ObjectId id)
        {
            Context.Bucket.Delete(id);
        }

        public async void CleanBucket()
        {
            var batch = 100;
            var filter = Builders<GridFSFileInfo>.Filter.And(
                Builders<GridFSFileInfo>.Filter.Lt(x => x.Metadata["retain_data_untill"], DateTime.UtcNow));
            var sort = Builders<GridFSFileInfo>.Sort.Descending(x => x.UploadDateTime);
            var options = new GridFSFindOptions
            {
                Limit = batch,
                Sort = sort
            };

            var again = false;

            do
            {
                using (var cursor = await Context.Bucket.FindAsync(filter, options))
                {
                    var fileInfos = (await cursor.ToListAsync());
                    again = fileInfos.Count() == batch;
                    Task.WaitAll(fileInfos.Select(fileInfo => Context.Bucket.DeleteAsync(fileInfo.Id)).ToArray());
                }
            }
            while (again);
        }

        public async Task<byte[]> Download(ObjectId id)
        {
            if (id == ObjectId.Empty)
                return new byte[0];
            else
                return await Context.Bucket.DownloadAsBytesAsync(id);
        }

        public Task<ObjectId> Upload(byte[] source, Session session)
        {
            var options = new GridFSUploadOptions
            {
                Metadata = new BsonDocument
                {
                    { "session", session.Id },
                    { "retain_data_untill", session.RetainDataUntil }
                }
            };

            return Context.Bucket.UploadFromBytesAsync("filename", source, options);
        }
    }
}
