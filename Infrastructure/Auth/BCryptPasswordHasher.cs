using Application.Interfaces.Services;

namespace Infrastructure.Auth
{
    public class BCryptPasswordHasher : IPasswordHasher
    {
        public string Hash(string password) 
            => BCrypt.Net.BCrypt.HashPassword(password);
        public bool Verify(string hashedPassword, string hash) 
            => BCrypt.Net.BCrypt.Verify(hashedPassword, hash);
    }
}
