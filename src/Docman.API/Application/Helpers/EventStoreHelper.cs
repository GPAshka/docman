using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Docman.API.Application.Dto;
using Docman.API.Application.Dto.Events;
using Docman.API.Application.Extensions;
using Docman.Domain;
using Docman.Infrastructure.EventStore;
using LanguageExt;

namespace Docman.API.Application.Helpers
{
    public static class EventStoreHelper
    {
        public static Func<string, EventDto, Task> SaveEvent =>
            async (connectionString, eventDto) =>
            {
                await EventsRepository.AddEvent(connectionString, eventDto.Id, eventDto);
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
                        DocumentRejectedEventDto rejectedEventDto => rejectedEventDto.ToEvent(),
                        DocumentUpdatedEventDto updatedEventDto => updatedEventDto.ToEvent(),
                        UserCreatedEventDto userCreatedEventDto => userCreatedEventDto.ToEvent(),
                        _ => new Error($"Unknown event DTO type: {dto.GetType().Name}")
                    });
                return events.Traverse(x => x);
            };
    }
}