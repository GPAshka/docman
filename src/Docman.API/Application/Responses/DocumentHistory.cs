using System;
using System.Collections.Generic;
using System.Linq;
using Docman.Domain;
using Docman.Domain.DocumentAggregate;
using Docman.Domain.DocumentAggregate.Events;

namespace Docman.API.Application.Responses
{
    public class DocumentHistory
    {
        public string Status { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}