using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Slipka
{
    public class ProxyStore
    {
        public ProxyStore(ISessionRepository sessionRepository)
        {
            Proxies = new List<Proxy>();
            SessionRepository = sessionRepository;
        }

        private List<Proxy> Proxies { get; set; }
        private ISessionRepository SessionRepository;

        public Proxy this[string id]
            => Proxies.FirstOrDefault(x=>x.Session.Id == id);

        public void Add(Proxy proxy)
        {
            Proxies.Add(proxy);
            SessionRepository.AddSession(proxy.Session);
        }

        public void Remove(string id)
        {
            var proxy = Proxies.FirstOrDefault(x => x.Session.Id == id);
            if (proxy != null)
            {
                proxy.Dispose();
                SessionRepository.UpdateSession(proxy.Session);
            }
            Proxies.RemoveAll(x => x.Session.Id == id);
        }
        public IEnumerable<Proxy> All => Proxies.Select(x => x);
    }
}
