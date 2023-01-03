using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace ESE.Clients.API.Application.Events
{
    public class ClientEventHandler : INotificationHandler<ClientRegisteredEvent>
    {
        public Task Handle(ClientRegisteredEvent notification, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
