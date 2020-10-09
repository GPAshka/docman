using System;
using Docman.API.Controllers;
using Docman.API.EventStore;
using Docman.Infrastructure.EventStore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Configuration;
using static LanguageExt.Prelude;

namespace Docman.API.Infrastructure
{
    public class DocumentsControllerActivator : IControllerActivator
    {
        private readonly IConfiguration _configuration;
        
        public DocumentsControllerActivator(IConfiguration configuration)
        {
            _configuration = configuration;
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
            var readEvents = par(EventStoreHelper.ReadEvents, connectionString);
            var saveEvent = par(EventStoreHelper.AddEvent, connectionString);

            return new DocumentsController(readEvents, saveEvent);
        }
    }
}