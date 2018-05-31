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
        public Proxy(Session value, IFileRepository fileRepository, IMessageRepository messageRepository)
        {
            Session = value;
            FileRepository = fileRepository;
            MessageRepository = messageRepository;
        }

        public Session Session { get; }
        private IWebHost Host { get; set; }
        private IFileRepository FileRepository { get; }
        private IMessageRepository MessageRepository { get; }

        public void Dispose()
        {
            Host.Dispose();
        }

        public void Init()
        {
            Host = new WebHostBuilder()
              .ConfigureServices(s => { s.AddSingleton(Session); })
              .ConfigureServices(s => { s.AddSingleton(FileRepository); })
              .ConfigureServices(s => { s.AddSingleton(MessageRepository); })
              .UseKestrel()
              .UseUrls($"http://*:{Session.ProxyPort}") 
              .UseStartup<ProxyStartup>()
              .Build();
            Host.Start();
        }
    }
}
