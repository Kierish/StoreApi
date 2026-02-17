using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StoreApi.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 3)]
        public required string Name { get; set; }

        public List<Tag>? Tags { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public required int CategoryId { get; set; }    
        
        public Category? Category { get; set; }
        
        [Required]
        [Range(0.1, (double)decimal.MaxValue)]
        [Column(TypeName = "decimal(18,2)")]
        public required decimal Price { get; set; }

        public ProductSeo? ProductSeo { get; set; }
    }
}
