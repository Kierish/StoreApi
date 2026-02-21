using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop.Infrastructure;
using StoreApi.Data;
using StoreApi.DTOs;
using StoreApi.Exceptions;
using StoreApi.Interfaces;
using StoreApi.Models;

namespace StoreApi.Services
{
    public class AccountService : IAccountSevice
    {
        private readonly AppDbContext _appDbContext;
        private readonly IAuthService _authService;

        public AccountService(AppDbContext appDbContext,
            IAuthService authService)
        {
            _appDbContext = appDbContext;
            _authService = authService;
        }

        public async Task<string> LoginUser(LoginDataDto dto)
        {
            var appUser = await _appDbContext.Users.FirstOrDefaultAsync(us => us.Email == dto.Email);

            if (appUser is null)
                throw new UnauthorizedAccessException("Invalid email or password.");


            string userPasswordHash = appUser.PasswordHash;
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(dto.Password, userPasswordHash);

            if (!isPasswordValid)
                throw new UnauthorizedAccessException("Invalid email or password.");

            return _authService.GenerateToken(appUser);
        }

        public async Task<string> RegisterUser(RegisterDataDto dto)
        {
            bool isUserExists = await _appDbContext.Users
                .AnyAsync(us => us.Email == dto.Email ||
                us.UserName == dto.UserName ||
                us.PhoneNumber == dto.PhoneNumber);

            if (isUserExists)
                throw new BadRequestException($"User already exists.");

            var newUser = new ApplicationUser
            {
                UserName = dto.UserName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
            };

            _appDbContext.Users.Add(newUser);
            await _appDbContext.SaveChangesAsync();

            return _authService.GenerateToken(newUser);
        }
    }
}
