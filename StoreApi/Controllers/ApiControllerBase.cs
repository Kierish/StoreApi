using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StoreApi.Common.Constants;
using StoreApi.Common.Primitives;
using StoreApi.DTOs;

namespace StoreApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Consumes("application/json")]
    [Produces("application/json")]
    public class ApiControllerBase<T> : ControllerBase where T : ApiControllerBase<T>
    {
        protected readonly ILogger<T> _logger;

        protected ApiControllerBase(ILogger<T> logger)
        {
            _logger = logger;
        }

        protected ActionResult HandleFailure<TDto>(Result<TDto> result)
        {
            if (result.IsSuccess)
                throw new InvalidOperationException("Cannot handle failure for a successful result.");

            _logger.LogWarning("Request failed. Code: {ErrorCode}. Reason: {ErrorMessage}",
                result.Error.Code, result.Error.Message);

            var statusCode = result.Error.Type switch
            {
                ErrorType.Validation => StatusCodes.Status400BadRequest,
                ErrorType.Failure => StatusCodes.Status400BadRequest,
                ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
                ErrorType.Forbidden => StatusCodes.Status403Forbidden,
                ErrorType.NotFound => StatusCodes.Status404NotFound,
                ErrorType.Conflict => StatusCodes.Status409Conflict,
                _ => StatusCodes.Status500InternalServerError
            };

            return Problem(
                statusCode: statusCode,
                title: result.Error.Code,    
                detail: result.Error.Message 
            );
        }
    }
}
