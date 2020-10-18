using Docman.API.Infrastructure;
using Docman.Infrastructure.PostgreSql;
using Docman.Infrastructure.Repositories;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Converters;
using static LanguageExt.Prelude;

namespace Docman.API
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
            services.AddControllers()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.Converters.Add(new StringEnumConverter());
                });

            services.AddMediatR(typeof(Startup));
            services.AddSingleton<IControllerActivator>(serviceProvider =>
                new DocumentsControllerActivator(Configuration, serviceProvider));
            
            services.AddDocumentRepositoryFunctions(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }

    internal static class CustomExtensions
    {
        public static IServiceCollection AddDocumentRepositoryFunctions(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration["PostgreSqlConnectionString"];

            services.AddSingleton<DocumentRepository.AddDocument>(par(DocumentPostgreSqlRepository.AddDocument,
                connectionString).Invoke);

            return services;
        }   
    }
}