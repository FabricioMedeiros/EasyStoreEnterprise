using FluentValidation.Results;

namespace ESE.Core.Messages.Itegration
{
    public class ResponseMessage : Message
    {
       public ValidationResult ValidationResult { get; set; }
    }
}
