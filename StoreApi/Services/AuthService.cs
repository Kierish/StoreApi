using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using StoreApi.Interfaces;
using StoreApi.Models.Identity;
using StoreApi.Settings;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace StoreApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly JwtSettings _jwtSettings;
        public AuthService(IOptions<JwtSettings> jwtOptions) 
        {
            _jwtSettings = jwtOptions.Value;
        }
        public string GenerateToken(ApplicationUser user)
        {
            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
            };

            var mySecretKey = _jwtSettings.SecretKey; 
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(mySecretKey));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var issuer = _jwtSettings.Issuer;
            var audience = _jwtSettings.Audience;

            var expirationTime = _jwtSettings.AccessTokenExpirationMinutes;

            var tokenObject = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expirationTime),
                signingCredentials: creds
                );

            var handler = new JwtSecurityTokenHandler();
            var tokenString = handler.WriteToken(tokenObject);

            return tokenString;
        }

        public RefreshToken GenerateRefreshToken(ApplicationUser user)
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            string token = Convert.ToBase64String(randomNumber);

            var expirationTime = _jwtSettings.RefreshTokenExpirationDays;

            var newRefreshToken = new RefreshToken
            {
                Token = token,
                DateAdded = DateTime.UtcNow,
                DateExpire = DateTime.UtcNow.AddDays(expirationTime),
                User = user
            };

            return newRefreshToken;
        }
    }
}
