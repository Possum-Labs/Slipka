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
        public ProxiesController(IOptions<ProxySettings> settings, ProxyStore store, IFileRepository fileRepository, IMessageRepository messageRepository)
        {
            Settings = settings.Value;
            Store = store;
            FileRepository = fileRepository;
            MessageRepository = messageRepository;
        }

        private ProxySettings Settings { get; }
        private ProxyStore Store { get; }
        private IFileRepository FileRepository { get; }
        private IMessageRepository MessageRepository { get; }

        Random rand = new Random(Guid.NewGuid().GetHashCode());

        // POST: api/Proxies
        [HttpPost]
        public Session Post([FromBody] Session value)
        {
            var session = new Session {
                TargetHost = value.TargetHost,
                TargetPort = value.TargetPort,
            };
            session.InternalId = new MongoDB.Bson.ObjectId();
            session.Calls = new List<Call>();
            lock (Store)
            {
                session.ProxyPort = GetNewPort(value);
                var proxy = new Proxy(session, FileRepository, MessageRepository);
                proxy.Init();
                Store.Add(proxy);
            }

            return session;

    }

        private int GetNewPort(Session value)
        {
            int[] ports = Enumerable.Range(Settings.FirstPort, Settings.LastPort - Settings.FirstPort).ToArray();
            var available = ports.Except(Store.All.Select(x => x.Session.ProxyPort)).ToArray();
            return available.ElementAt(rand.Next(available.Count()));
        }



        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(string id)
        {
            lock (Store)
            {
                Store.Remove(id);
             }
        }

        [HttpPut("{id}/record")]
        public Session PutRecord(string id, [FromBody] CallTemplate call)
        {
            var session = 
                Store[id].Session;
            session.RecordedCalls.Add(call);
            return session;
        }

        [HttpPut("{id}/intercept")]
        public Session PutIntercept(string id, [FromBody] CallTemplate call)
        {
            var session =
                Store[id].Session;
            session.InterceptedCalls.Add(call);
            return session;
        }
    }
}
