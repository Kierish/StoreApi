using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StoreApi.Models.Identity
{
    public class RefreshToken
    {
        [Key]
        public int Id { get; set; }
        public required string Token { get; set; }
        public required DateTime DateAdded { get; set; }
        public required DateTime DateExpire { get; set; }

        [ForeignKey(nameof(ApplicationUser))]
        public int UserId { get; set; }
        public required ApplicationUser User { get; set; }   
    }
}
