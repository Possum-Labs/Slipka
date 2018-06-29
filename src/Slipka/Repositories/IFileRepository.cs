using MongoDB.Bson;
using Slipka.DomainObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Slipka.Repositories
{
    public interface IFileRepository
    {
        Task<byte[]> Download(ObjectId id);
        Task<ObjectId> Upload(byte[] file, Session session);
        void Delete(ObjectId id);
    }
}
