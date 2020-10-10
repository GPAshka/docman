using System;
using System.Collections.Generic;
using Docman.Domain.DocumentAggregate;
using Docman.Domain.Events;
using LanguageExt;
using static LanguageExt.Prelude;

namespace Docman.Domain
{
    public static class DocumentStateTransition
    {
        public static Document CreateDocument(this DocumentCreatedEvent evt) 
            => new Document(evt.EntityId, evt.Number, evt.Description, DocumentStatus.Draft);

        public static Document Apply(this Document document, Event evt)
        {
            return evt switch
            {
                DocumentApprovedEvent approvedEvent => document.Approve(approvedEvent.Comment),
                FileAddedEvent fileAddedEvent => document.WithFile(fileAddedEvent.FileId, fileAddedEvent.Name,
                    fileAddedEvent.Description),
                DocumentSentForApprovalEvent _ => document.WithStatus(DocumentStatus.WaitingForApproval)
            };
        }

        public static Option<Document> From(IEnumerable<Event> history)
            => history.Match(
                Empty: () => None,
                More: (createdEvent, otherEvents) => Some(
                    otherEvents.Aggregate(
                        seed: CreateDocument((DocumentCreatedEvent) createdEvent),
                        func: (state, evt) => state.Apply(evt)
                    )));

        public static Validation<Error, (Document Document, DocumentApprovedEvent Event)> Approve(
            this Document document, string comment)
        {
            if (document.Status != DocumentStatus.WaitingForApproval)
                return new Error($"Document should have {DocumentStatus.WaitingForApproval} status");

            return Comment.Create(comment)
                .Map(c => new DocumentApprovedEvent(document.Id, c))
                .Map(evt => (document.Apply(evt), evt));
        }

        public static Validation<Error, (Document Document, FileAddedEvent Event)> AddFile(this Document document,
            string fileName, string fileDescription)
        {
            if (document.Status != DocumentStatus.Draft)
                return new Error($"Document should have {DocumentStatus.Draft} status");

            return File.Create(Guid.NewGuid(), fileName, fileDescription)
                .Map(file => new FileAddedEvent(document.Id, file.Id, file.Name, file.Description, DateTime.UtcNow))
                .Map(evt => (document.Apply(evt), evt));
        }

        public static Validation<Error, (Document Document, DocumentSentForApprovalEvent Event)> SendForApproval(
            this Document document)
        {
            if (document.Status != DocumentStatus.Draft)
                return new Error($"Document should have {DocumentStatus.Draft} status");

            return Validation<Error, DocumentSentForApprovalEvent>
                .Success(new DocumentSentForApprovalEvent(document.Id))
                .Map(evt => (document.Apply(evt), evt));
        }

        public static DocumentStatus GetStatus(this Event @event)
            => @event switch
            {
                DocumentCreatedEvent _ => DocumentStatus.Draft,
                DocumentApprovedEvent _ => DocumentStatus.Approved
            };
    }
}