using MediatR;

namespace Docman.API.Application.Dto.Events
{
    public class DocumentSentForApprovalEventDto : EventDto, INotification
    {
    }
}