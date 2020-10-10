using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Docman.API.Dto.Events;
using Docman.API.Extensions;
using Docman.Domain;
using Docman.Domain.Events;
using Docman.Infrastructure.EventStore;
using LanguageExt;

namespace Docman.API.EventStore
{
    public static class EventStoreHelper
    {
        public static Action<string, Event> AddEvent =>
            (connectionString, @event) =>
            {
                object eventDto = @event switch
                {
                    DocumentCreatedEvent createdEvent => createdEvent.ToDto(),
                    DocumentApprovedEvent approvedEvent => approvedEvent.ToDto(),
                    FileAddedEvent fileAddedEvent => fileAddedEvent.ToDto(),
                    DocumentSentForApprovalEvent sentForApprovalEvent => sentForApprovalEvent.ToDto()
                };

                EventsRepository.AddEvent(connectionString, @event.EntityId, eventDto);
            };
        
        public static Func<string, Guid, Task<Validation<Error, IEnumerable<Event>>>> ReadEvents =>
            async (connectionString, entityId) =>
            {
                var events = await EventsRepository.ReadEvents(connectionString, entityId)
                    .MapT(dto => dto switch
                    {
                        DocumentCreatedEventDto createdEventDto => createdEventDto.ToEvent(),
                        DocumentApprovedEventDto approvedEventDto => approvedEventDto.ToEvent(),
                        FileAddedEventDto fileAddedEventDto => fileAddedEventDto.ToEvent(),
                        DocumentSentForApprovalEventDto sentForApprovalEventDto => sentForApprovalEventDto.ToEvent(),
                        _ => new Error($"Unknown event DTO type: {dto.GetType().Name}")
                    });
                return events.Traverse(x => x);
            };
    }
}