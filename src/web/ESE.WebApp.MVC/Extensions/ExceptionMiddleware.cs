using System.Net;
using System.Threading.Tasks;
using ESE.WebApp.MVC.Services;
using Microsoft.AspNetCore.Http;
using Polly.CircuitBreaker;

namespace ESE.WebApp.MVC.Extensions
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private static IAuthService _authenticationService;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext, IAuthService authenticationService)
        {
            _authenticationService = authenticationService;

            try
            {
                await _next(httpContext);
            }
            catch (CustomHttpRequestException ex)
            {
                HandleRequestExceptionAsync(httpContext, ex);
            }
            catch (BrokenCircuitException)
            {
                HandleCircuitBreakerExceptionAsync(httpContext);
            }
        }

        private static void HandleRequestExceptionAsync(HttpContext context, CustomHttpRequestException httpRequestException)
        {
            if (httpRequestException.StatusCode == HttpStatusCode.Unauthorized)
            {
                if (_authenticationService.TokenExpired())
                {
                    if (_authenticationService.RefreshTokenIsValid().Result)
                    {
                        context.Response.Redirect(context.Request.Path);
                        return;
                    }
                }

                _authenticationService.Logout();
                context.Response.Redirect($"/login?ReturnUrl={context.Request.Path}");
                return;
            }

            context.Response.StatusCode = (int)httpRequestException.StatusCode;
        }

        private static void HandleCircuitBreakerExceptionAsync(HttpContext context)
        {
            context.Response.Redirect("/system-unavailable");
        }
    }
}