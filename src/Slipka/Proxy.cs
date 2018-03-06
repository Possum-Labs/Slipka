using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        public IFileRepository FileRepository;

        public Proxy(Session value, IFileRepository fileRepository)
        {
            Session = value;
            FileRepository = fileRepository;
        }

        public void Dispose()
        {
            Host.Dispose();
        }

        public void Init()
        {
            Host = new WebHostBuilder()
              .ConfigureServices(s => { s.AddSingleton(Session); })
              .ConfigureServices(s => { s.AddSingleton(FileRepository); })
              .UseKestrel()
              .UseUrls($"http://*:{Session.ProxyPort}") 
              .UseStartup<ProxyStartup>()
              .Build();
            Host.Start();
        }
    }
}
