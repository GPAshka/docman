using System.Dynamic;
using System.Net;
using System.Net.Http;
using Docman.API;
using Docman.IntegrationTests.Infrastructure;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using WebMotions.Fake.Authentication.JwtBearer;
using Xunit;

namespace Docman.IntegrationTests
{
    public class BaseIntegrationTests : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        protected readonly HttpClient _client;

        public BaseIntegrationTests(CustomWebApplicationFactory<Startup> factory)
        {
            _client = factory.WithWebHostBuilder(builder =>
            {
                builder
                    .UseTestServer()
                    .ConfigureTestServices(services =>
                        services.AddAuthentication(FakeJwtBearerDefaults.AuthenticationScheme).AddFakeJwtBearer());
            }).CreateClient();
            
            dynamic data = new ExpandoObject();
            data.sub = "XUUqekBGJfPhwbK9XnyYi8nmulk1";
            _client.SetFakeBearerToken((object)data);
        }
    }
}