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

        public byte[] Download(ObjectId id)
        {
            return _context.Bucket.DownloadAsBytes(id);
        }

        public ObjectId Upload(byte[] source, Session session)
        {
            var options = new GridFSUploadOptions
            {
                Metadata = new BsonDocument
                {
                    { "session", session.Id }
                }
            };

            return _context.Bucket.UploadFromBytes("filename", source, options);
        }
    }
}
