using System.ComponentModel.DataAnnotations;

namespace Domain.Models.Auth
{
    public class ApplicationUser
    {
        public Guid Id { get; set; } = Guid.CreateVersion7();
        public required string Email { get; set; }
        public required string PasswordHash { get; set; }
        public required string UserName { get; set; }
        public required string PhoneNumber { get; set; }
        public required string Role { get; set; }
    }
}
