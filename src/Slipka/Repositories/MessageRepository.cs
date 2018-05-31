using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Slipka
{
    public class MessageRepository: IMessageRepository
    {
        private readonly SlipkaContext _context = null;

        public MessageRepository(IOptions<MongoSettings> settings)
        {
            _context = new SlipkaContext(settings);
        }

        // query after Id or InternalId (BSonId value)
        //
        public async Task<Message> GetMessage(ObjectId id)
        {
            if (id == ObjectId.Empty)
                return null;

            return await _context.Messages
                            .Find(Message => Message.InternalId == id)
                            .FirstOrDefaultAsync();
        }

        public async Task AddMessage(Message item)
        {
            await _context.Messages.InsertOneAsync(item);
        }

        public async Task<bool> RemoveMessage(string id)
        {
            DeleteResult actionResult
                = await _context.Messages.DeleteOneAsync(
                    Builders<Message>.Filter.Eq("Id", id));

            return actionResult.IsAcknowledged
                && actionResult.DeletedCount > 0;
        }

        public async Task UpdateMessage(Message item)
        {
            await _context.Messages
                .ReplaceOneAsync(n => n.InternalId.Equals(item.InternalId)
                        , item
                        , new UpdateOptions { IsUpsert = true });
        }

    }
}
