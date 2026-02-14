using System.ComponentModel.DataAnnotations;

namespace StoreApi.DTOs
{
    public class ProductUpdateDto
    {
        [StringLength(100, MinimumLength = 3)]
        public string? Name { get; set; }
        public string? Category { get; set; }
        [Range(0.1, double.MaxValue)]
        public decimal? Price { get; set; }
    }
}
