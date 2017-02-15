using System;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using Blaster.Server.Implementation;
using Microsoft.AspNetCore.Http;

namespace Blaster.Server
{
    public class BlasterWebServer
    {
        private readonly BlasterProducer _producer;

        public BlasterWebServer(BlasterProducer producer)
        {
            _producer = producer;
        }

        public async Task Handle(HttpContext context)
        {
            Trace.WriteLine("received!");

            var contentLength = context.Request.ContentLength.GetValueOrDefault();
            if (contentLength == 0)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                await context.Response.WriteAsync("Invalid request");
                return;
            }

            await context.Response.WriteAsync("Hello World!");
        }
    }
}
