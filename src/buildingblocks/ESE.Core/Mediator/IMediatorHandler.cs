using ESE.Core.Messages;
using FluentValidation.Results;
using System.Threading.Tasks;

namespace ESE.Core.Mediator
{
    public interface IMediatorHandler
    {
        Task PublishEvent<T>(T events) where T : Event;
        Task<ValidationResult> SendCommand<T>(T commands) where T : Command;
    }
}
