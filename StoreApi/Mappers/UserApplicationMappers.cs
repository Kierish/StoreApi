using StoreApi.DTOs;
using StoreApi.Models.Identity;

namespace StoreApi.Mappers
{
    public static class UserApplicationMappers
    {
        public static ApplicationUser ToEntity(this RegisterDataDto dto)
        {
            return new ApplicationUser
            {
                UserName = dto.UserName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = UserRoles.Customer
            };
        }
    }
}
