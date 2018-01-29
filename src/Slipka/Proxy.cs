using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Slipka
{
    public class Proxy : IDisposable
    {
        public Session Session;
        public IWebHost Host;

        public Proxy(Session value)
        {
            Session = value;
        }

        public void Dispose()
        {
            Host.Dispose();
        }

        public void Init()
        {
            Host = new WebHostBuilder()
              .ConfigureServices(s => { s.AddSingleton(Session); })
              .UseKestrel()
              .UseUrls($"http://*:{Session.ProxyPort}") 
              .UseStartup<ProxyStartup>()
              .Build();
            Host.Start();
        }
    }
}
