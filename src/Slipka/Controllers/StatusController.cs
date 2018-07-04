using Microsoft.AspNetCore.Mvc;
using Slipka.Configuration;
using Slipka.Proxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Slipka.Controllers
{
    [Route("api/Status")]
    public class StatusController : Controller
    {

        public StatusController(ProxySettings proxySettings, MongoSettings mongoSettings, ProxyStore store, Status status)
        {
            ProxySettings = proxySettings;
            MongoSettings = mongoSettings;
            Store = store;
            Status = status;
        }

        private ProxySettings ProxySettings { get; }
        private MongoSettings MongoSettings { get; }
        private ProxyStore Store { get; }
        private Status Status { get; }

        [HttpGet("ProxySettings")]
        public ProxySettings GetProxySettings()
            => ProxySettings;

        [HttpGet("MongoSettings")]
        public MongoSettings GetMongoSettings()
            => MongoSettings;

        [HttpGet("Status")]
        public Status GetStatus()
            => Status;

        public class StateElement
        {
            public string Name { get; set; }
            public string Id { get; set; }
            public int Port { get; set; }
        }

        [HttpGet("State")]
        public IEnumerable<StateElement> GetState()
            => Store.All.Select(p => new StateElement { Id = p.Id, Name = p.Name, Port = p.ProxyPort });

    }
}
