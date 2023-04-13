using ESE.Core.Communication;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;
using System.Linq;

namespace ESE.WebAPI.Core.Controllers
{
    [ApiController]
    public abstract class MainController : Controller
    {
        protected ICollection<string> Erros = new List<string>();

        protected ActionResult CustomResponse(object result = null)
        {
            if (!HasError())
            {
                return Ok(result);
            }

            return BadRequest(new ValidationProblemDetails(new Dictionary<string, string[]>
            {
                { "Messages", Erros.ToArray() }
            }));
        }

        protected ActionResult CustomResponse(ModelStateDictionary modelState)
        {
            var erros = modelState.Values.SelectMany(e => e.Errors);

            foreach (var error in erros)
            {
                AddProcessingError(error.ErrorMessage);
            }

            return CustomResponse();
        }

        protected ActionResult CustomResponse(ValidationResult validationResult)
        {
            foreach (var error in validationResult.Errors)
            {
                AddProcessingError(error.ErrorMessage);
            }

            return CustomResponse();
        }

        protected ActionResult CustomResponse(ResponseResult result)
        {
            ResponseHasErrors(result);

            return CustomResponse();
        }

        protected bool ResponseHasErrors(ResponseResult response)
        {
            if (response == null || !response.Errors.Messages.Any()) return false;

            foreach (var message in response.Errors.Messages)
            {
                AddProcessingError(message);
            }

            return true;
        }

        protected bool HasError()
        {
            return Erros.Any();
        }

        protected void AddProcessingError(string error)
        {
            Erros.Add(error);
        }

        protected void ClearProcessingError()
        {
            Erros.Clear();
        }
    }
}
