using Microsoft.AspNetCore.Mvc;
using System;

namespace TheatreManagementSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class ApiControllerBase : ControllerBase
    {
        protected IActionResult HandleException(Exception ex)
        {
            // Log the exception here
            return StatusCode(500, new { message = "An error occurred", error = ex.Message });
        }

        protected IActionResult NotFound(string message)
        {
            return NotFound(new { message });
        }

        protected IActionResult BadRequest(string message)
        {
            return BadRequest(new { message });
        }

        protected IActionResult Created(string routeName, object routeValues, object value)
        {
            return CreatedAtRoute(routeName, routeValues, value);
        }

        protected IActionResult Forbidden(string message)
        {
            return StatusCode(403, new { message });
        }
    }
}
