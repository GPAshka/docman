using System;
using Docman.API.Application.Helpers;
using Docman.API.Controllers;
using Docman.Domain;
using Docman.Infrastructure.PostgreSql;
using Docman.Infrastructure.Repositories;
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

            if (type == typeof(DocumentsController))
                return NewDocumentsController();
            
            return type == typeof(DocumentFilesController)
                ? NewDocumentFilesController()
                : Activator.CreateInstance(type);
        }

        public void Release(ControllerContext context, object controller)
        {
            if (controller is IDisposable disposable) 
                disposable.Dispose();
        }

        private DocumentsController NewDocumentsController()
        {
            //TODO catch exceptions while reading configuration
            var eventStoreConnectionString = _configuration["EventStoreConnectionString"];
            var postgresConnectionString = _configuration["PostgreSqlConnectionString"];

            var readEvents = par(EventStoreHelper.ReadEvents, eventStoreConnectionString);
            var saveAndPublish = ConstructSaveAndPublishEventFunc();

            var documentExistsByNumber = par(DocumentPostgresRepository.DocumentExistsByNumberAsync,
                postgresConnectionString);
            var getDocument = par(DocumentPostgresRepository.GetDocumentByIdAsync, postgresConnectionString);

            return new DocumentsController(readEvents, saveAndPublish,
                new DocumentRepository.DocumentExistsByNumber(documentExistsByNumber),
                new DocumentRepository.GetDocumentById(getDocument));
        }

        private DocumentFilesController NewDocumentFilesController()
        {
            var eventStoreConnectionString = _configuration["EventStoreConnectionString"];
            
            var readEvents = par(EventStoreHelper.ReadEvents, eventStoreConnectionString);
            var saveAndPublish = ConstructSaveAndPublishEventFunc();

            return new DocumentFilesController(readEvents, saveAndPublish);
        }

        private Action<Event> ConstructSaveAndPublishEventFunc()
        {
            var eventStoreConnectionString = _configuration["EventStoreConnectionString"];
            var mediator = _serviceProvider.GetRequiredService<IMediator>();
            
            var saveEvent = par(EventStoreHelper.SaveEvent, eventStoreConnectionString);
            
            var saveAndPublish = par(HelperFunctions.SaveAndPublish, dto => saveEvent(dto),
                dto => mediator.Publish(dto));
            return saveAndPublish;
        }
    }
}