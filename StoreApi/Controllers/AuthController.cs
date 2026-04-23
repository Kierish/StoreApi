using Application.DTOs;
using Application.Exceptions;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using StoreApi.Filters;

namespace StoreApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAccountService _accService;

        public AuthController(IAccountService accService)
        {
            _accService = accService;
        }

        [HttpPost("login-user")]
        [ServiceFilter(typeof(ValidationFilter<LoginDataDto>))]
        public async Task<ActionResult<AuthResponseDto>> Login(LoginDataDto dto)
        {
            if (dto is null)
                throw new BadRequestException("Invalid data.");

            return Ok(await _accService.LoginUserAsync(dto));
        }

        [HttpPost("register-user")]
        [ServiceFilter(typeof(ValidationFilter<RegisterDataDto>))]
        public async Task<ActionResult<AuthResponseDto>> Register(RegisterDataDto dto)
        {
            if (dto is null)
                throw new BadRequestException("Invalid data.");

            return Ok(await _accService.RegisterUserAsync(dto));
        }

        [HttpPost("refresh")]
        [ServiceFilter(typeof(ValidationFilter<AuthRequestDto>))]
        public async Task<ActionResult<AuthResponseDto>> RefreshToken(AuthRequestDto dto)
        {
            if (dto is null)
                throw new BadRequestException("Invalid data.");

            return Ok(await _accService.RefreshTokenAsync(dto));
        }
    }
}
