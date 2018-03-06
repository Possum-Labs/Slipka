using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Proxy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Slipka
{
    public class ProxyStartup
    {
        public ProxyStartup(IConfiguration configuration, Session session , IFileRepository fileRepository)
        {
            Configuration = configuration;
            Session = session;
            Target = new HostString(Session.TargetHost, Session.TargetPort);
            FileRepository = fileRepository;

    }

    private IFileRepository FileRepository { get; }
        public IConfiguration Configuration { get; }
        public Session Session { get; }
        public HostString Target { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var handler = new ProxyHandler(Session, FileRepository);
            services.AddMvc();
            services.AddProxy( options => 
            {
                options.MessageHandler = handler;
                options.PrepareRequest = (originalRequest, message) =>
                {
                    message.Headers.Add("X-Forwarded-Host", originalRequest.Host.Host);
                    return Task.FromResult(0);
                };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.MapWhen(context => true, builder => builder.RunProxy(new ProxyOptions
            {
                Scheme = "http",
                Host = Target
            }));

            app.UseMvc();
        }

 
    }
}
