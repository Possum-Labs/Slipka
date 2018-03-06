using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Slipka
{
    public interface ISessionRepository
    {
        Task<IEnumerable<Session>> GetAllSessions();
        Task<Session> GetSession(string id);

        Task AddSession(Session item);

        Task<bool> RemoveSession(string id);
        
        Task UpdateSession(Session item);
    }
}
