using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System;

namespace TheatreManagementSystem.Controllers
{
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ErrorController : ControllerBase
    {
        [Route("/error")]
        public IActionResult Error()
        {
            var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
            var exception = context?.Error;

            var statusCode = 500;
            var message = "An unexpected error occurred";

            if (exception is UnauthorizedAccessException)
            {
                statusCode = 403;
                message = "You don't have permission to access this resource";
            }
            else if (exception != null)
            {
                message = exception.Message;
            }

            return StatusCode(statusCode, new { status = statusCode, message });
        }

        [Route("/error/{statusCode}")]
        public IActionResult HttpStatusCodeHandler(int statusCode)
        {
            var message = statusCode switch
            {
                404 => "Resource not found",
                403 => "Forbidden",
                401 => "Unauthorized",
                400 => "Bad request",
                _ => "An error occurred"
            };

            return StatusCode(statusCode, new { status = statusCode, message });
        }
    }
}