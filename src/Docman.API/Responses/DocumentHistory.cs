using System;
using Docman.Domain;
using Docman.Domain.Events;

namespace Docman.API.Responses
{
    public class DocumentHistory
    {
        public string Status { get; set; }
        public DateTime TimeStamp { get; set; }
        
        public static DocumentHistory EventToDocumentHistory(Event @event)
            => new DocumentHistory
            {
                Status = @event.GetStatus().ToString(),
                TimeStamp = @event.TimeStamp
            };
    }
}