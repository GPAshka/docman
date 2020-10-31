using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace Docman.IntegrationTests
{
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            var path = Assembly.GetAssembly(typeof(CustomWebApplicationFactory<>))?.Location;

            builder.UseContentRoot(Path.GetDirectoryName(path));
            builder.ConfigureAppConfiguration(cb =>
            {
                cb.AddJsonFile("appsettings.json")
                    .AddEnvironmentVariables();
            });
            
            base.ConfigureWebHost(builder);
        }
    }
}