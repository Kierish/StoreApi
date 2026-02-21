using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StoreApi.Data;
using StoreApi.Interfaces;
using StoreApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace StoreApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        public AuthService(IConfiguration configuration) 
        {
            _configuration = configuration;
        }
        public string GenerateToken(ApplicationUser user)
        {
            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
            };

            var mySecretKey = _configuration["JwtSettings:SecretKey"] 
                ?? throw new InvalidOperationException("JWT Secret Key is missing from configuration.");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(mySecretKey));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var issuer = _configuration["JwtSettings:Issuer"] 
                ?? throw new InvalidOperationException("JWT Issuer is missing from configuration.");
            var audience = _configuration["JwtSettings:Audience"] 
                ?? throw new InvalidOperationException("JWT Audience is missing from configuration.");

            var tokenObject = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(10),
                signingCredentials: creds
                );

            var handler = new JwtSecurityTokenHandler();
            var tokenString = handler.WriteToken(tokenObject);

            return tokenString;
        }
    }
}
