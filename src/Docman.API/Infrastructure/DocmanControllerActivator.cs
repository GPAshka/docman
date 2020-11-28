using System;
using System.Threading.Tasks;
using Docman.API.Application.Helpers;
using Docman.API.Controllers;
using Docman.Domain;
using Docman.Infrastructure.PostgreSql;
using Docman.Infrastructure.Repositories;
using LanguageExt;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using static LanguageExt.Prelude;
using Unit = LanguageExt.Unit;

namespace Docman.API.Infrastructure
{
    public class DocmanControllerActivator : IControllerActivator
    {
        private readonly IServiceProvider _serviceProvider;

        private readonly IConfiguration _configuration;
        private readonly string _eventStoreConnectionString;
        private readonly string _postgresConnectionString;
        
        public DocmanControllerActivator(IConfiguration configuration, IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            
            //TODO catch exceptions while reading configuration
            _configuration = configuration;
            _eventStoreConnectionString = configuration["EventStoreConnectionString"];
            _postgresConnectionString = configuration["PostgreSqlConnectionString"];
        }
        
        public object Create(ControllerContext context)
        {
            var type = context.ActionDescriptor.ControllerTypeInfo.AsType();

            if (type == typeof(DocumentsController))
                return NewDocumentsController();

            if (type == typeof(UsersController))
                return NewUsersController();

            if (type == typeof(DocumentFilesController))
                return NewDocumentFilesController();
            
            return Activator.CreateInstance(type)!;
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

            var getCurrentUserId = par(UserHelper.GetCurrentUserId,
                new UserRepository.GetUserByFirebaseId(par(UserPostgresRepository.GetUserByFirebaseId,
                    _postgresConnectionString)));
            
            return new DocumentsController(readEvents, saveAndPublish,
                new DocumentRepository.DocumentExistsByNumber(documentExistsByNumber),
                new DocumentRepository.GetDocumentById(getDocument), getCurrentUserId);
        }

        private DocumentFilesController NewDocumentFilesController()
        {
            var readEvents = par(EventStoreHelper.ReadEvents, _eventStoreConnectionString);
            var saveAndPublish = ConstructSaveAndPublishEventFunc();

            var getDocument = par(DocumentPostgresRepository.GetDocumentByIdAsync, _postgresConnectionString);
            var getFile = par(DocumentPostgresRepository.GetFileByIdAsync, _postgresConnectionString);
            var getFiles = par(DocumentPostgresRepository.GetFilesAsync, _postgresConnectionString);
            
            var getCurrentUserId = par(UserHelper.GetCurrentUserId,
                new UserRepository.GetUserByFirebaseId(par(UserPostgresRepository.GetUserByFirebaseId,
                    _postgresConnectionString)));

            return new DocumentFilesController(readEvents, saveAndPublish, new DocumentRepository.GetFile(getFile),
                new DocumentRepository.GetFiles(getFiles), getCurrentUserId,
                new DocumentRepository.GetDocumentById(getDocument));
        }

        private UsersController NewUsersController()
        {
            var createFirebaseUser = par(FirebaseHelper.CreateUser, _configuration["FirebaseWebApiKey"]);
            var signInUser = par(FirebaseHelper.SignInUser, _configuration["FirebaseWebApiKey"]);
            var saveAndPublish = ConstructSaveAndPublishEventFunc();

            return new UsersController(createFirebaseUser, saveAndPublish, signInUser);
        }

        private Func<Event, Task<Validation<Error, Unit>>> ConstructSaveAndPublishEventFunc()
        {
            var mediator = _serviceProvider.GetRequiredService<IMediator>();
            
            var saveEvent = par(EventStoreHelper.SaveEvent, _eventStoreConnectionString);
            return par(EventHelper.SaveAndPublish, async dto => await saveEvent(dto),
                async dto => await mediator.Publish(dto));
        }
    }
}