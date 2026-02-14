using System.ComponentModel.DataAnnotations;

namespace StoreApi.DTOs
{
    public class ProductCreateDto
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, MinimumLength = 3)]
        public required string Name { get; set; }
        public required string Category { get; set; }
        [Required]
        [Range(0.1, double.MaxValue)]
        public required decimal Price { get; set; }
    }
}
