using Blaster.Server.Implementation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Blaster.Server
{
    public class Startup
    {
        private BlasterWebServer _blasterServer;

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IApplicationLifetime lifeTime)
        {
            loggerFactory.AddConsole();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            var producer = new BlasterProducer(new QueueOptions
            {
                BaseLocation = @"C:\Temp",
                CancellationToken = lifeTime.ApplicationStopping
            });

            _blasterServer = new BlasterWebServer(producer);

            app.Run(async (context) =>
            {
                await _blasterServer.Handle(context);
            });
        }
    }
}
