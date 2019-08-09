using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Slipka.DomainObjects;
using Slipka.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Slipka.Proxy
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
            try
            {
                var task = Task.Run(async () => { await Host.StopAsync(); });
                task.Wait();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed to stop a proxy: {e}");
            }
            try
            { 
                Host.Dispose();
            }
            catch(Exception e)
            {
                Console.WriteLine($"Failed to dispose a proxy, might never have started: {e}");
            }
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
