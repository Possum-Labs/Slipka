using MongoDB.Bson;
using Slipka.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Slipka.Repositories
{
    public interface IMessageRepository
    {
        Task<Message> GetMessage(ObjectId id);

        Task AddMessage(Message item);

        Task<bool> RemoveMessage(string id);
        
        Task UpdateMessage(Message item);
    }
}
