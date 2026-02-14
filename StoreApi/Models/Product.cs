using System.ComponentModel.DataAnnotations;

namespace StoreApi.Models
{
    public class Product
    {
        [Range(1, int.MaxValue)]
        public int Id { get; set; }
        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, MinimumLength = 3)]
        public required string Name { get; set; }
        public required string Category { get; set; }
        [Required]
        [Range(0.1, (double)decimal.MaxValue)]
        public required decimal Price { get; set; }
    }
}
