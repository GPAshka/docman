using System;
using MediatR;

namespace Docman.API.Application.Dto.Events
{
    public class FileAddedEventDto : EventDto, INotification
    {
        public Guid FileId { get; set; }
        public string FileName { get; set; }
        public string FileDescription { get; set; }
    }
}