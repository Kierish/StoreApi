using Microsoft.AspNetCore.Mvc;
using StoreApi.Data;
using StoreApi.DTOs;
using StoreApi.Services;
using StoreApi.Models;
using StoreApi.Exceptions;
using Microsoft.EntityFrameworkCore;
using StoreApi.Interfaces;
using Microsoft.JSInterop.Infrastructure;

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
        public async Task<ActionResult<AuthResponseDto>> Login(LoginDataDto user)
        { 
            if (user is null)
                throw new BadRequestException("Incalid data.");

            string token = await _accService.LoginUser(user);

            return Ok(new AuthResponseDto(token));
        }

        [HttpPost("reg-user")]
        public async Task<ActionResult<AuthResponseDto>> Register(RegisterDataDto dto)
        {
            if (dto is null)
                throw new BadRequestException("Invalid data.");

            string token = await _accService.RegisterUser(dto);

            return Ok(new AuthResponseDto(token));
        }
    }
}
