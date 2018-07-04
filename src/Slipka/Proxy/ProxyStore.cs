using Slipka.Configuration;
using Slipka.DomainObjects;
using Slipka.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Slipka.Proxy
{
    public class ProxyStore
    {
        public ProxyStore(ISessionRepository sessionRepository, ProxySettings settings)
        {
            Proxies = new List<Proxy>();
            SessionRepository = sessionRepository;

            LastUpdated = new Dictionary<string, Record>();

            System.Timers.Timer asyncUpdater = new System.Timers.Timer();
            asyncUpdater.Elapsed += AsyncUpdater_Elapsed;
            asyncUpdater.Interval = settings.ProxyPersistanceLoop;
            asyncUpdater.Enabled = true;
        }

        private void AsyncUpdater_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            foreach (var session in this.All)
            {
                var state = session.State();
                var record = LastUpdated[session.Id];
                if (session.LeaveProxyOpenUntil < DateTime.UtcNow)
                {
                    Remove(session.Id);
                    continue;
                }
                if (record.State == state)
                    continue;
                PersistSession(session);
            }
        }

        private void PersistSession(Session session)
        {
            var record = LastUpdated[session.Id];
            record.State = session.State();
            record.LastUpdate = DateTime.Now;
            SessionRepository.UpdateSession(session);
        }

        private List<Proxy> Proxies { get; }        
        private ISessionRepository SessionRepository { get; }
        private Dictionary<string, Record> LastUpdated { get; }

        private class Record
        {
            public int State { get; set; }
            public DateTime LastUpdate { get; set; }
        }

        public Proxy this[string id]
        {
            get
            {
                lock (Proxies)
                {
                    return Proxies.FirstOrDefault(x => x.Session.Id == id);
                }
            }
        }

        public void Add(Proxy proxy)
        {
            lock (Proxies)
            {
                Proxies.Add(proxy);
                var id = " "+proxy.Session.Id;
                LastUpdated.Add(proxy.Session.Id, new Record());
            }
            SessionRepository.AddSession(proxy.Session);
        }

        public void Remove(string id)
        {
            Proxy proxy;
            lock (Proxies)
            {
                proxy = Proxies.FirstOrDefault(x => x.Session.Id == id);
                Proxies.RemoveAll(x => x.Session.Id == id);
            }
            if (proxy != null)
            {
                proxy.Dispose();
                SessionRepository.UpdateSession(proxy.Session);
            }
        }

        public void SaveSession(object sender, SessionEventArgs e)
        {
            PersistSession(e.Session);
        }

        public IEnumerable<Session> All
        {
            get
            {
                lock (Proxies)
                {
                    return Proxies.Select(x => x.Session).ToList();
                }
            }
        }
    }
}
