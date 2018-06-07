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
        public Proxy(Session value, IFileRepository fileRepository, IMessageRepository messageRepository, EventHandler<SessionEventArgs> saveSession)
        {
            Session = value;
            FileRepository = fileRepository;
            MessageRepository = messageRepository;
            SaveSession = saveSession;
        }

        public Session Session { get; }
        private IWebHost Host { get; set; }
        private IFileRepository FileRepository { get; }
        private IMessageRepository MessageRepository { get; }
        private EventHandler<SessionEventArgs> SaveSession { get; }

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
              .ConfigureServices(s=> { s.AddSingleton(SaveSession); })
              .UseKestrel()
              .UseUrls($"http://*:{Session.ProxyPort}") 
              .UseStartup<ProxyStartup>()
              .Build();
            Host.Start();
        }
    }
}
