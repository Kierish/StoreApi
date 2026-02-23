using System.ComponentModel.DataAnnotations;

namespace StoreApi.Models.Identity
{
    public class ApplicationUser
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public required string Email { get; set; }
        [Required]
        public required string PasswordHash { get; set; }
        public required string UserName { get; set; }
        public required string PhoneNumber { get; set; }
        public required string Role { get; set; }
    }
}
