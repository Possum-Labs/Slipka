using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Slipka.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Slipka
{
    public class SessionRepository : ISessionRepository
    {
        private readonly SlipkaContext _context = null;

        public SessionRepository(MongoSettings settings)
        {
            _context = new SlipkaContext(settings);
        }

        public async Task<IEnumerable<Session>> GetAllSessions()
        {
            return await _context.Sessions
                    .Find(_ => true).ToListAsync();
        }

        // query after Id or InternalId (BSonId value)
        //
        public async Task<Session> GetSession(string id)
        {
            ObjectId internalId = GetInternalId(id);
            return await _context.Sessions
                            .Find(Session => Session.InternalId == internalId
                                    || Session.Id == id)
                            .FirstOrDefaultAsync();
        }

        private ObjectId GetInternalId(string id)
        {
            ObjectId internalId;
            if (!ObjectId.TryParse(id, out internalId))
                internalId = ObjectId.Empty;

            return internalId;
        }

        public async Task AddSession(Session item)
        {
            await _context.Sessions.InsertOneAsync(item);
        }

        public async Task<bool> RemoveSession(string id)
        {
            DeleteResult actionResult
                = await _context.Sessions.DeleteOneAsync(
                    Builders<Session>.Filter.Eq("Id", id));

            return actionResult.IsAcknowledged
                && actionResult.DeletedCount > 0;
        }

        public async Task<bool> RemoveSessions()
        {
            DeleteResult actionResult
                = await _context.Sessions.DeleteManyAsync(Builders<Session>.Filter.Empty);

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

            await _context.Sessions
                .ReplaceOneAsync(n => n.InternalId.Equals(item.InternalId)
                        , copy
                        , new UpdateOptions { IsUpsert = true });

        }

    }
}
