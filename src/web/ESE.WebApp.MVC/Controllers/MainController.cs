using ESE.WebApp.MVC.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace ESE.WebApp.MVC.Controllers
{
    public class MainController : Controller
    {
        protected bool HasErrorsResponse(ResponseResult response)
        {
            if (response != null && response.Errors.Messages.Any())
            {
                foreach (var message in response.Errors.Messages)
                {
                    ModelState.AddModelError(string.Empty, message);
                }
                
                return true;
            }

            return false;
        }

        protected void AddProcessingError(string mensagem)
        {
            ModelState.AddModelError(string.Empty, mensagem);
        }

        protected bool IsValid()
        {
            return ModelState.ErrorCount == 0;
        }
    }
}
