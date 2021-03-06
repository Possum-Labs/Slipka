﻿using Slipka.Configuration;
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
            Parallel.ForEach(this.All, session =>
            { 
                var state = session.State();
                var record = LastUpdated[session.Id];
                if(record.RemoveAwaited)
                {
                    InternalRemove(session.Id);
                    return;
                }
                if(record.Remove)
                {
                    record.RemoveAwaited = true;
                    return;
                }
                if (session.LeaveProxyOpenUntil < DateTime.UtcNow)
                {
                    Remove(session.Id);
                    return;
                }
                PersistSession(session);
            });
        }

        private void PersistSession(Session session)
        {
            try
            {
                var record = LastUpdated[session.Id];
                record.LastUpdate = DateTime.Now;
                SessionRepository.UpdateSession(session);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private List<Proxy> Proxies { get; }        
        private ISessionRepository SessionRepository { get; }
        private Dictionary<string, Record> LastUpdated { get; }

        private class Record
        {
            public DateTime LastUpdate { get; set; }
            public bool Remove { get; set; }
            public bool RemoveAwaited { get; set; }
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
                LastUpdated.Add(proxy.Session.Id, new Record());
            }
            SessionRepository.AddSession(proxy.Session);
        }

        public void Remove(string id)
        {
            PersistSession(this[id].Session);
            LastUpdated[id].Remove = true;
        }

        private void InternalRemove(string id)
        { 
            Proxy proxy;
            lock (Proxies)
            {
                proxy = Proxies.FirstOrDefault(x => x.Session.Id == id);
                Proxies.RemoveAll(x => x.Session.Id == id);
                LastUpdated.Remove(id);
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
