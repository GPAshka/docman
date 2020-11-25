using System;
using MediatR;

namespace Docman.API.Application.Dto.Events
{
    public record FileAddedEventDto : EventDto, INotification
    {
        public Guid FileId { get; }
        public string FileName { get; }
        public string FileDescription { get; }

        public FileAddedEventDto(Guid id, DateTime timeStamp, Guid fileId, string fileName, string fileDescription) :
            base(id, timeStamp)
        {
            FileId = fileId;
            FileName = fileName;
            FileDescription = fileDescription;
        }
    }
}