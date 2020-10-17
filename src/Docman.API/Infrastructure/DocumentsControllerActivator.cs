using System;
using Docman.API.Application.Dto;
using Docman.API.Application.Helpers;
using Docman.API.Controllers;
using Docman.API.Extensions;
using Docman.Domain;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using static LanguageExt.Prelude;

namespace Docman.API.Infrastructure
{
    public class DocumentsControllerActivator : IControllerActivator
    {
        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _serviceProvider;
        
        public DocumentsControllerActivator(IConfiguration configuration, IServiceProvider serviceProvider)
        {
            _configuration = configuration;
            _serviceProvider = serviceProvider;
        }
        
        public object Create(ControllerContext context)
        {
            var type = context.ActionDescriptor.ControllerTypeInfo.AsType();
            return type == typeof(DocumentsController) ? NewDocumentsController() : Activator.CreateInstance(type);
        }

        public void Release(ControllerContext context, object controller)
        {
            if (controller is IDisposable disposable) 
                disposable.Dispose();
        }

        private DocumentsController NewDocumentsController()
        {
            //TODO catch exceptions while reading configuration
            var connectionString = _configuration["EventStoreConnectionString"];
            var mediator = _serviceProvider.GetRequiredService<IMediator>();
            
            var readEvents = par(EventStoreHelper.ReadEvents, connectionString);
            var saveEvent = par(EventStoreHelper.SaveEvent, connectionString);

            var saveAndPublish = par(Functions.SaveAndPublish, dto => mediator.Publish(dto),
                dto => saveEvent(dto));
            
            return new DocumentsController(readEvents, saveAndPublish);
        }
    }
}