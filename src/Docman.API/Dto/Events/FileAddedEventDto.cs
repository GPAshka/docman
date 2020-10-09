using System;

namespace Docman.API.Dto.Events
{
    public class FileAddedEventDto
    {
        public string DocumentId { get; set; }
        public string FileId { get; set; }
        public string FileName { get; set; }
        public string FileDescription { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}