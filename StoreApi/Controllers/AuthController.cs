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

        [HttpPost("login-user")]
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

        [HttpPost("register-user")]
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

        [HttpPost("refresh")]
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
