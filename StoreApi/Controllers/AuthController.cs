using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoreApi.DTOs.Auth;
using StoreApi.Services.Auth;

namespace StoreApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ApiControllerBase<AuthController>
    {
        private readonly IAccountService _accService;

        public AuthController(IAccountService accService,
            ILogger<AuthController> logger) : base(logger) 
        {
            _accService = accService;
        }

        /// <summary>
        /// Authenticates a user and issues JWT and Refresh tokens.
        /// </summary>
        /// <param name="dto">The user's login credentials.</param>
        /// <returns>Access token and Refresh token.</returns>
        /// <response code="200">Successfully authenticated.</response>
        /// <response code="401">Invalid email or password.</response>
        [HttpPost("login-user")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDataDto dto)
        {
            var result = await _accService.LoginUserAsync(dto);

            if (!result.IsSuccess)
            {
                return HandleFailure(result);
            }

            _logger.LogInformation("User {Email} logged in successfully.", dto.Email);

            return Ok(result.Data);
        }

        /// <summary>
        /// Registers a new customer account.
        /// </summary>
        /// <param name="dto">The registration data including email, username, phone, and password.</param>
        /// <returns>Access token and Refresh token.</returns>
        /// <response code="200">User registered successfully.</response>
        /// <response code="401">Invalid email or password.</response>
        [HttpPost("register-user")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterDataDto dto)
        {
            var result = await _accService.RegisterUserAsync(dto);

            if (!result.IsSuccess)
            {
                return HandleFailure(result);
            }

            _logger.LogInformation("New user registered successfully with email {Email}.", dto.Email);

            return Ok(result.Data);
        }

        /// <summary>
        /// Refreshes an expired JWT using a valid Refresh token.
        /// </summary>
        /// <param name="dto">The expired JWT and the active Refresh token.</param>
        /// <returns>A new pair of Access and Refresh tokens.</returns>
        /// <response code="200">Tokens refreshed successfully.</response>
        /// <response code="401">Invalid email or password.</response>
        [HttpPost("refresh")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<AuthResponseDto>> RefreshToken([FromBody] AuthRequestDto dto)
        {
            var result = await _accService.RefreshTokenAsync(dto);

            if (!result.IsSuccess)
            {
                return HandleFailure(result);
            }

            _logger.LogInformation("Tokens refreshed successfully.");

            return Ok(result.Data);
        }
    }
}
