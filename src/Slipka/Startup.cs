using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Http;
using GraphQL.Server.Transports.AspNetCore;
using GraphQL.Types;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Slipka.Configuration;
using Slipka.Graphql;
using Slipka.Repositories;
using Slipka.Proxy;
using System.IO;

namespace Slipka
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var status = new Status();
            var configurationFactory = new ConfigurationFactory(Configuration, status);

            services.AddSingleton<IDependencyResolver>(s => new FuncDependencyResolver(s.GetRequiredService));

            services.AddMvc();
            services.AddSingleton(status);
            services.AddSingleton(configurationFactory.Create<MongoSettings>());
            services.AddSingleton(configurationFactory.Create<ProxySettings>());
            services.AddSingleton(new SlipkaContext(configurationFactory.Create<MongoSettings>(),status)); //start the DB configuration
            services.AddTransient<ISessionRepository, SessionRepository>();
            services.AddTransient<IMessageRepository, MessageRepository>();
            services.AddTransient<IFileRepository, FileRepository>();
            services.AddSingleton<ProxyStore>();
            services.AddSingleton<GridFsCleanupLoop>();


            services.AddSingleton<CallTemplateType>();
            services.AddSingleton<CallType>();
            services.AddSingleton<HeaderType>();
            services.AddSingleton<MessageTemplateType>();
            services.AddSingleton<MessageType>();
            services.AddSingleton<MethodEnum>();
            services.AddSingleton<SessionType>();
            services.AddSingleton<SlipkaMutation>();
            services.AddSingleton<SlipkaQuery>();
            services.AddSingleton<ISchema, SlipkaSchema>();

            services.AddGraphQLHttp();

            services.Configure<ExecutionOptions>(options =>
            {
                options.EnableMetrics = true;
                options.ExposeExceptions = true;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseDeveloperExceptionPage();

            app.UseGraphQLHttp<ISchema>(new GraphQLHttpOptions());

            app.UseGraphiQl();

            app.UseMvcWithDefaultRoute();
            app.UseDefaultFiles();
            app.UseStaticFiles();

        }

 
    }
}
