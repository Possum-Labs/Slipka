using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Slipka.Configuration;
using Slipka.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Slipka.Repositories
{
    public class MessageRepository: IMessageRepository
    {
        private SlipkaContext Context { get; }

        public MessageRepository(SlipkaContext context)
        {
            Context = context;
        }

        // query after Id or InternalId (BSonId value)
        //
        public async Task<Message> GetMessage(ObjectId id)
        {
            if (id == ObjectId.Empty)
                return null;

            return await Context.Messages
                            .Find(Message => Message.InternalId == id)
                            .FirstOrDefaultAsync();
        }

        public async Task AddMessage(Message item)
        {
            await Context.Messages.InsertOneAsync(item);
        }

        public async Task<bool> RemoveMessage(string id)
        {
            DeleteResult actionResult
                = await Context.Messages.DeleteOneAsync(
                    Builders<Message>.Filter.Eq("Id", id));

            return actionResult.IsAcknowledged
                && actionResult.DeletedCount > 0;
        }

        public async Task UpdateMessage(Message item)
        {
            await Context.Messages
                .ReplaceOneAsync(n => n.InternalId.Equals(item.InternalId)
                        , item
                        , new UpdateOptions { IsUpsert = true });
        }

    }
}
