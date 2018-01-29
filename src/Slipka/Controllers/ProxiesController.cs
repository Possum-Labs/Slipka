using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Slipka.Controllers
{
    [Produces("application/json")]
    [Route("api/Proxies")]
    public class ProxiesController : Controller
    {
        public ProxiesController(IOptions<ProxySettings> settings, IOptions<ProxyStore> store)
        {
            Settings = settings.Value;
            Store = store.Value;
        }

        private readonly ProxySettings Settings;
        private readonly ProxyStore Store;
        Random rand = new Random();

        // GET: api/Proxies
        [HttpGet]
        public IEnumerable<Session> Get()
        {
            lock (Store)
            {
                return Store.Proxies.Select(x => x.Session);
            }
        }

        // GET: api/Proxies/5
        [HttpGet("{id}", Name = "Get")]
        public Session Get(string id)
        {
            var guid = Guid.Parse(id);
            lock (Store)
            {
                return Store.Proxies.First(x => x.Session.PublicId == guid).Session;
            }
        }
        
        // POST: api/Proxies
        [HttpPost]
        public Session Post([FromBody]Session value)
        {
            value.Id = new MongoDB.Bson.ObjectId();
            value.PublicId = Guid.NewGuid();
            value.Calls = new List<Call>();
            lock (Store)
            {
                value.ProxyPort = GetNewPort(value);
                var proxy = new Proxy(value);
                proxy.Init();
                Store.Proxies.Add(proxy);
            }

            return value;

    }

        private int GetNewPort(Session value)
        {
            int[] ports = Enumerable.Range(Settings.FirstPort, Settings.LastPort - Settings.FirstPort).ToArray();
            var available = ports.Except(Store.Proxies.Select(x => x.Session.ProxyPort)).ToArray();
            return available.ElementAt(rand.Next(available.Count()));
        }



        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(string id)
        {
            var guid = Guid.Parse(id);
            lock (Store)
            {
                foreach( var p in Store.Proxies.Where(x => x.Session.PublicId == guid))
                    p.Dispose();
                Store.Proxies.RemoveAll(x => x.Session.PublicId == guid);
            }
        }
    }
}
