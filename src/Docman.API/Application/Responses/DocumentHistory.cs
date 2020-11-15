using System;
using LanguageExt;

namespace Docman.API.Application.Responses
{
    public class DocumentHistory : Record<DocumentHistory>
    {
        public string Status { get; }
        public DateTime TimeStamp { get; }
        
        public DocumentHistory(string status, DateTime timeStamp)
        {
            Status = status;
            TimeStamp = timeStamp;
        }
    }
}