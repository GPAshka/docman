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
        private readonly IServiceProvider _serviceProvider;

        private readonly string _eventStoreConnectionString;
        private readonly string _postgresConnectionString;
        
        public DocumentsControllerActivator(IConfiguration configuration, IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            
            //TODO catch exceptions while reading configuration
            _eventStoreConnectionString = configuration["EventStoreConnectionString"];
            _postgresConnectionString = configuration["PostgreSqlConnectionString"];
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
            var readEvents = par(EventStoreHelper.ReadEvents, _eventStoreConnectionString);
            var saveAndPublish = ConstructSaveAndPublishEventFunc();

            var documentExistsByNumber = par(DocumentPostgresRepository.DocumentExistsByNumberAsync,
                _postgresConnectionString);
            var getDocument = par(DocumentPostgresRepository.GetDocumentByIdAsync, _postgresConnectionString);

            return new DocumentsController(readEvents, saveAndPublish,
                new DocumentRepository.DocumentExistsByNumber(documentExistsByNumber),
                new DocumentRepository.GetDocumentById(getDocument));
        }

        private DocumentFilesController NewDocumentFilesController()
        {
            var readEvents = par(EventStoreHelper.ReadEvents, _eventStoreConnectionString);
            var saveAndPublish = ConstructSaveAndPublishEventFunc();

            var getFile = par(DocumentPostgresRepository.GetFileByIdAsync, _postgresConnectionString);
            var getFiles = par(DocumentPostgresRepository.GetFilesAsync, _postgresConnectionString);

            return new DocumentFilesController(readEvents, saveAndPublish, new DocumentRepository.GetFile(getFile),
                new DocumentRepository.GetFiles(getFiles));
        }

        private Action<Event> ConstructSaveAndPublishEventFunc()
        {
            var mediator = _serviceProvider.GetRequiredService<IMediator>();
            
            var saveEvent = par(EventStoreHelper.SaveEvent, _eventStoreConnectionString);
            return par(HelperFunctions.SaveAndPublish, dto => saveEvent(dto),
                dto => mediator.Publish(dto));
        }
    }
}