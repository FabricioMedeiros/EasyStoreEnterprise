using ESE.Core.Messages;
using FluentValidation.Results;
using MediatR;
using System.Threading.Tasks;

namespace ESE.Core.Mediator
{
    class MediatorHandler : IMediatorHandler
    {
        private readonly IMediator _mediator;

        public MediatorHandler(IMediator mediator)
        {
            _mediator = mediator;
        }
        public async Task PublishEvent<T>(T events) where T : Event
        {
            await _mediator.Publish(events);
        }

        public async Task<ValidationResult> SendCommand<T>(T commands) where T : Command
        {
            return await _mediator.Send(commands);
        }
    }
}
