using Application.Common;
using Microsoft.AspNetCore.Mvc;

namespace SubastaYa.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class ApiControllerBase : ControllerBase
    {
        
        protected ActionResult<T> HandleResult<T>(Result<T> result)
        {
            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return MapError(result.ErrorType, result.ErrorMessage);
        }

        protected ActionResult HandleResult(Result result)
        {
            if (result.IsSuccess)
            {
                return Ok();
            }

            return MapError(result.ErrorType, result.ErrorMessage);
        }

        private ObjectResult MapError(ErrorType errorType, string? message)
        {
            var statusCode = errorType switch
            {
                ErrorType.Validation => StatusCodes.Status400BadRequest,
                ErrorType.NotFound => StatusCodes.Status404NotFound,
                ErrorType.Conflict => StatusCodes.Status409Conflict,
                ErrorType.InsufficientFunds => StatusCodes.Status422UnprocessableEntity,
                _ => StatusCodes.Status500InternalServerError
            };

            return Problem(detail: message, statusCode: statusCode);
        }
        protected int GetUserId()
        {
            if (Request.Headers.TryGetValue("UserId", out var raw) &&
                int.TryParse(raw, out var id))
            {
                return id;
            }
            return 0;
        }
    }
}
