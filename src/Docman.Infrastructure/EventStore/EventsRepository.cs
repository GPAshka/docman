using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Newtonsoft.Json;

namespace Docman.Infrastructure.EventStore
{
    public static class EventsRepository
    {
        public static Action<string, string, object> AddEvent =>
            async (connectionString, entityId, eventDto) =>
            {
                using var connection = await CreateAndOpenConnection(connectionString);
                await connection.AppendToStreamAsync(entityId, ExpectedVersion.Any, Map(eventDto));
            };

        public static Func<string, Guid, Task<IEnumerable<object>>> ReadEvents =>
            async (connectionString, entityId) =>
            {
                var events = new List<object>();

                StreamEventsSlice currentSlice;
                long nextSliceStart = StreamPosition.Start;

                using var connection = await CreateAndOpenConnection(connectionString);

                do
                {
                    currentSlice =
                        await connection.ReadStreamEventsForwardAsync(entityId.ToString(), nextSliceStart, 200, false);
                    nextSliceStart = currentSlice.NextEventNumber;

                    events.AddRange(currentSlice.Events.Select(Map));
                } while (!currentSlice.IsEndOfStream);

                return events;
            };

        private static async Task<IEventStoreConnection> CreateAndOpenConnection(string connectionString)
        {
            var settings = ConnectionSettings.Create()
                .EnableVerboseLogging()
                .UseConsoleLogger()
                .DisableTls()
                .Build();

            var connectionStringUri = new Uri(connectionString);
            var connection = EventStoreConnection.Create(settings, connectionStringUri);
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

        private static object Map(ResolvedEvent resolvedEvent)
        {
            var meta = JsonConvert.DeserializeObject<EventMetaData>(
                Encoding.UTF8.GetString(resolvedEvent.Event.Metadata));
            var eventType = Type.GetType(meta.EventType);
            
            return JsonConvert.DeserializeObject(Encoding.UTF8.GetString(resolvedEvent.Event.Data), eventType);
        }
    }
}