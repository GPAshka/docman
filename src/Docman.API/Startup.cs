using Docman.API.Infrastructure;
using Docman.Infrastructure.PostgreSql;
using Docman.Infrastructure.PostgreSql.Migrations;
using Docman.Infrastructure.Repositories;
using FluentMigrator.Runner;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
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

            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.IncludeErrorDetails = true;
                    options.Authority = "https://securetoken.google.com/docman-a427d";
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = "https://securetoken.google.com/docman-a427d",
                        ValidateAudience = true,
                        ValidAudience = "docman-a427d",
                        ValidateLifetime = true
                    };
                });

            services.AddMediatR(typeof(Startup));
            services.AddSingleton<IControllerActivator>(serviceProvider =>
                new DocmanControllerActivator(Configuration, serviceProvider));
            
            var postgresConnectionString = Configuration["PostgreSqlConnectionString"];
            services.AddSingleton(new DocumentRepository.AddDocument(par(DocumentPostgresRepository.AddDocument,
                postgresConnectionString)));
            services.AddSingleton(new DocumentRepository.UpdateDocument(par(DocumentPostgresRepository.UpdateDocument,
                postgresConnectionString)));
            services.AddSingleton(new DocumentRepository.UpdateDocumentStatus(
                par(DocumentPostgresRepository.UpdateDocumentStatus, postgresConnectionString)));
            services.AddSingleton(new DocumentRepository.AddFile(par(DocumentPostgresRepository.AddFile,
                postgresConnectionString)));
            services.AddSingleton(new DocumentRepository.ApproveDocument(par(DocumentPostgresRepository.ApproveDocument,
                postgresConnectionString)));
            services.AddSingleton(new DocumentRepository.RejectDocument(par(DocumentPostgresRepository.RejectDocument,
                postgresConnectionString)));
            
            services.AddFluentMigratorCore().ConfigureRunner(rb => rb
                .AddPostgres()
                .WithGlobalConnectionString(postgresConnectionString)
                .ScanIn(typeof(CreateDocumentsTable).Assembly).For.Migrations());
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

            app.UseAuthentication();
            
            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

            app.Migrate();
        }
    }

    internal static class CustomExtensions
    {
        public static IApplicationBuilder Migrate(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var runner = scope.ServiceProvider.GetService<IMigrationRunner>();
            runner.MigrateUp();
            return app;
        }
    }
}