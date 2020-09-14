using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Docman.Domain;
using Docman.Domain.Events;
using Docman.Infrastructure.EventDto;
using EventStore.ClientAPI;
using LanguageExt;
using Newtonsoft.Json;

namespace Docman.Infrastructure.EventStore
{
    public class EventsRepository
    {
        private readonly Uri _connectionString;

        public EventsRepository(Uri connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task AddEvent(string entityId, object @event)
        {
            using var connection = await CreateAndOpenConnection();
            await connection.AppendToStreamAsync(entityId, ExpectedVersion.Any, Map(@event));
        }

        public async Task<Validation<Error, IEnumerable<Event>>> ReadEvents(Guid entityId)
        {
            var events = new List<Validation<Error, Event>>();

            StreamEventsSlice currentSlice;
            long nextSliceStart = StreamPosition.Start;
            
            using var connection = await CreateAndOpenConnection();
            
            do
            {
                currentSlice =
                    await connection.ReadStreamEventsForwardAsync(entityId.ToString(), nextSliceStart, 200, false);
                nextSliceStart = currentSlice.NextEventNumber;

                events.AddRange(currentSlice.Events.Select(Map));
            } while (!currentSlice.IsEndOfStream);

            return events.Traverse(x => x);
        }

        private async Task<IEventStoreConnection> CreateAndOpenConnection()
        {
            var settings = ConnectionSettings.Create()
                .EnableVerboseLogging()
                .UseConsoleLogger()
                .DisableTls()
                .Build();

            var connection = EventStoreConnection.Create(settings, _connectionString);
            await connection.ConnectAsync();
            return connection;
        }

        private static EventData Map(object @event)
        {
            var eventType = @event.GetType();
            
            var dataJson = JsonConvert.SerializeObject(@event);
            var data = Encoding.UTF8.GetBytes(dataJson);

            var meta = new EventMetaData(eventType.AssemblyQualifiedName);
            var metaJson = JsonConvert.SerializeObject(meta);
            var metadata = Encoding.UTF8.GetBytes(metaJson); 
            
            return new EventData(Guid.NewGuid(), eventType.Name, true, data, metadata);
        }

        private static Validation<Error, Event> Map(ResolvedEvent resolvedEvent)
        {
            var meta = JsonConvert.DeserializeObject<EventMetaData>(
                Encoding.UTF8.GetString(resolvedEvent.Event.Metadata));
            var eventType = Type.GetType(meta.EventType);

            if (eventType == null)
                return new Error($"Unknown event type: {meta.EventType}");

            var eventDto = JsonConvert.DeserializeObject(Encoding.UTF8.GetString(resolvedEvent.Event.Data), eventType);
            return eventDto switch
            {
                DocumentCreatedEventDto createdEventDto => createdEventDto.ToEvent(),
                DocumentApprovedEventDto approvedEventDto => approvedEventDto.ToEvent(),
                _ => new Error($"Unknown event DTO type: {eventType}")
            };
        }
    }
}