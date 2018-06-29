using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Slipka.Configuration;
using Slipka.DomainObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Slipka.Repositories
{
    public class SessionRepository : ISessionRepository
    {
        private SlipkaContext Context { get; }

        public SessionRepository(SlipkaContext context)
        {
            Context = context;
        }

        public async Task<IEnumerable<Session>> GetAllSessions()
        {
            return await Context.Sessions
                    .Find(_ => true).ToListAsync();
        }

        // query after Id or InternalId (BSonId value)
        //
        public async Task<Session> GetSession(string id)
        {
            ObjectId internalId = GetInternalId(id);
            return await Context.Sessions
                            .Find(Session => Session.InternalId == internalId
                                    || Session.Id == id)
                            .FirstOrDefaultAsync();
        }

        private ObjectId GetInternalId(string id)
        {
            if (!ObjectId.TryParse(id, out ObjectId internalId))
                internalId = ObjectId.Empty;

            return internalId;
        }

        public async Task AddSession(Session item)
        {
            await Context.Sessions.InsertOneAsync(item);
        }

        public async Task<bool> RemoveSession(string id)
        {
            DeleteResult actionResult
                = await Context.Sessions.DeleteOneAsync(
                    Builders<Session>.Filter.Eq("Id", id));

            return actionResult.IsAcknowledged
                && actionResult.DeletedCount > 0;
        }

        public async Task<bool> RemoveSessions()
        {
            DeleteResult actionResult
                = await Context.Sessions.DeleteManyAsync(Builders<Session>.Filter.Empty);

            return actionResult.IsAcknowledged
                && actionResult.DeletedCount > 0;
        }

        public async Task UpdateSession(Session item)
        {
            Session copy;
            lock (item.Calls)
            {
                copy = new Session
                {
                    Calls = item.Calls.ToList(),
                    InternalId = item.InternalId,
                    Id = item.Id,
                    Name = item.Name,
                    ProxyPort = item.ProxyPort,
                    TargetHost = item.TargetHost,
                    TargetPort = item.TargetPort,
                    Tags = item.Tags.ToList(),
                    RecordedCalls = item.RecordedCalls,
                    InjectedCalls = item.InjectedCalls,
                    TaggedCalls = item.TaggedCalls,
                    Decorations = item.Decorations,
                };
            }

            await Context.Sessions
                .ReplaceOneAsync(n => n.InternalId.Equals(item.InternalId)
                        , copy
                        , new UpdateOptions { IsUpsert = true });

        }

    }
}
