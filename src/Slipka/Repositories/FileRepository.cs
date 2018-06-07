using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver.GridFS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Slipka
{
    public class FileRepository : IFileRepository
    {

        private readonly SlipkaContext _context = null;

        public FileRepository(IOptions<MongoSettings> settings)
        {
            _context = new SlipkaContext(settings);
        }

        public void Delete(ObjectId id)
        {
            _context.Bucket.Delete(id);
        }

        public async Task<byte[]> Download(ObjectId id)
        {
            if (id == ObjectId.Empty)
                return new byte[0];
            else
                return await _context.Bucket.DownloadAsBytesAsync(id);
        }

        public Task<ObjectId> Upload(byte[] source, Session session)
        {
            var options = new GridFSUploadOptions
            {
                Metadata = new BsonDocument
                {
                    { "session", session.Id }
                }
            };

            return _context.Bucket.UploadFromBytesAsync("filename", source, options);
        }
    }
}
