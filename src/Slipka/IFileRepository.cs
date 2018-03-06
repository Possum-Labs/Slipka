using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Slipka
{
    public interface IFileRepository
    {
        byte[] Download(ObjectId id);
        ObjectId Upload(byte[] file, Session session);
        void Delete(ObjectId id);
    }
}
