using Microsoft.AspNetCore.Mvc;
using StoreApi.DTOs;
using StoreApi.Exceptions;
using StoreApi.Interfaces.Services;

namespace StoreApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAccountSevice _accService;

        public AuthController(IAccountSevice accService) 
        {
            _accService = accService;
        }

        [HttpPost("login-user")]
        public async Task<ActionResult<AuthResponseDto>> Login(LoginDataDto dto)
        { 
            if (dto is null)
                throw new BadRequestException("Invalid data.");

            return Ok(await _accService.LoginUserAsync(dto));
        }

        [HttpPost("reg-user")]
        public async Task<ActionResult<AuthResponseDto>> Register(RegisterDataDto dto)
        {
            if (dto is null)
                throw new BadRequestException("Invalid data.");

            return Ok(await _accService.RegisterUserAsync(dto));
        }

        [HttpPost("refresh")]
        public async Task<ActionResult<AuthResponseDto>> RefreshToken(AuthRequestDto dto)
        {
            if (dto is null)
                throw new BadRequestException("Invalid data.");

            return Ok(await _accService.RefreshTokenAsync(dto));
        }
    }
}
