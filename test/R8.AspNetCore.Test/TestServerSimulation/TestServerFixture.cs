﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using R8.AspNetCore.WebTest;

using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net.Http;

using Xunit.Abstractions;

namespace R8.AspNetCore.Test.TestServerSimulation
{
    public class TestServerFixture : WebApplicationFactory<Startup>
    {
        public HttpClient Client { get; }
        public ITestOutputHelper Output { get; set; }

        public TestServerFixture()
        {
            // CreateServer(CreateWebHostBuilder());
        }

        [ExcludeFromCodeCoverage]
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                Output = null;
            }
        }

        protected override IWebHostBuilder CreateWebHostBuilder()
        {
            var builder = base.CreateWebHostBuilder()
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddXUnit(Output);
                });
            return builder;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                services.RemoveAll(typeof(IHostedService));
                services
                    .AddMvc()
                    .AddApplicationPart(typeof(Startup).Assembly);
            });

            builder.UseWebRoot(Directory.GetCurrentDirectory() + @"\www");
            builder.UseContentRoot(Directory.GetCurrentDirectory());
            builder.UseSolutionRelativeContentRoot(Directory.GetCurrentDirectory(), "R8.sln");
            builder.UseStartup<Startup>().UseEnvironment("Test");
            Output.WriteLine("Specified Directory: {0}", Directory.GetCurrentDirectory());
        }

        public TestServerFixture SetOutput(ITestOutputHelper output)
        {
            Output = output;
            return this;
        }
    }
}