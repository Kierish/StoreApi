using System.ComponentModel.DataAnnotations;

namespace StoreApi.Models
{
    public class ApplicationUser
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public required string Email { get; set; }
        [Required]
        public required string PasswordHash { get; set; }
        public string? UserName { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
