using System;
using System.Collections.Generic;
using Docman.Domain.DocumentAggregate.Errors;
using Docman.Domain.DocumentAggregate.Events;
using LanguageExt;

namespace Docman.Domain.DocumentAggregate
{
    public static class DocumentStateTransition
    {
        private static Document CreateDocument(this DocumentCreatedEvent evt)
            => new Document(new DocumentId(evt.EntityId), evt.Number, evt.Description, DocumentStatus.Draft);

        private static Validation<Error, Document> Apply(this Document document, Event evt)
        {
            return evt switch
            {
                DocumentApprovedEvent approvedEvent => document.Approve(approvedEvent.Comment),
                FileAddedEvent fileAddedEvent => document.AddFile(fileAddedEvent.FileId, fileAddedEvent.Name,
                    fileAddedEvent.Description),
                DocumentSentForApprovalEvent _ => document.WaitingForApproval(),
                DocumentRejectedEvent rejectedEvent => document.Reject(rejectedEvent.Reason),
                DocumentUpdatedEvent updatedEvent => document.Update(updatedEvent.Number, updatedEvent.Description)
            };
        }

        public static Validation<Error, Document> From(IEnumerable<Event> history, Guid documentId)
        {
            return history.Match(
                Empty: () => new DocumentNotFoundError(documentId), 
                More: (createdEvent, otherEvents) =>
                    otherEvents.Aggregate(
                        seed: Validation<Error, Document>.Success(CreateDocument((DocumentCreatedEvent) createdEvent)),
                        func: (state, evt) => state.Bind(doc =>
                            doc.Apply(evt))));
        }

        public static Validation<Error, (Document Document, DocumentUpdatedEvent Event)> Update(this Document document,
            string number, string description) =>
            DocumentUpdatedEvent.Create(document.Id, number, description)
                .Bind(evt => document.Apply(evt)
                    .Map(doc => (doc, evt)));
        

        public static Validation<Error, (Document Document, DocumentApprovedEvent Event)> Approve(
            this Document document, string comment) =>
            DocumentApprovedEvent.Create(document.Id, comment)
                .Bind(evt => document.Apply(evt)
                    .Map(doc => (doc, evt)));

        public static Validation<Error, (Document Document, DocumentRejectedEvent Event)> Reject(this Document document,
            string reason) =>
            DocumentRejectedEvent.Create(document.Id, reason)
                .Bind(evt => document.Apply(evt)
                    .Map(doc => (doc, evt)));

        public static Validation<Error, (Document Document, FileAddedEvent Event)> AddFile(this Document document,
            string fileName, string fileDescription) =>
            FileAddedEvent.Create(document.Id, fileName, fileDescription)
                .Bind(evt => document.Apply(evt)
                    .Map(doc => (doc, evt)));

        public static Validation<Error, (Document Document, DocumentSentForApprovalEvent Event)> SendForApproval(
            this Document document) =>
            Validation<Error, DocumentSentForApprovalEvent>
                .Success(new DocumentSentForApprovalEvent(document.Id))
                .Bind(evt => document.Apply(evt)
                    .Map(doc => (doc, evt)));
    }
}