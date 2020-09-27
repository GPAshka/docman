using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Docman.API.Commands;
using Docman.API.Controllers;
using Docman.Domain;
using Docman.Domain.Events;
using LanguageExt;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace Docman.UnitTests.Controllers
{
    public class DocumentControllerTests
    {
        private DocumentsController _documentsController;
        
        [Fact]
        public void TestCreateDocument()
        {
            //Arrange
            var command = new CreateDocumentCommand(Guid.Empty, "1234", "test");

            static Task<Validation<Error, IEnumerable<Event>>> ReadEvents(Guid id) =>
                Task.FromResult(Validation<Error, IEnumerable<Event>>.Success(new List<Event>()));

            static void SaveAndPublish(Event evt) { }

            _documentsController = new DocumentsController(ReadEvents, SaveAndPublish);
            
            //Act
            var result = _documentsController.CreateDocument(command);
            
            //Assert
            var createdResult = result as CreatedResult; 
            Assert.NotNull(createdResult);
            Assert.NotNull(createdResult.Location);
        }
    }
}