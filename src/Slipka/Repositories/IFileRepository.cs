using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Slipka
{
    public interface IFileRepository
    {
        Task<byte[]> Download(ObjectId id);
        Task<ObjectId> Upload(byte[] file, Session session);
        void Delete(ObjectId id);
    }
}
