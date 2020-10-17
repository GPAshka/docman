using System;

namespace Docman.API.Dto.Events
{
    public class FileAddedEventDto : EventDto
    {
        public string FileId { get; set; }
        public string FileName { get; set; }
        public string FileDescription { get; set; }
    }
}