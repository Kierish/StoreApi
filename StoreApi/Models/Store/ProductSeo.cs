using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StoreApi.Models.Store
{
    public class ProductSeo
    {
        [Key]
        [ForeignKey(nameof(Product))]
        public int Id { get; set; }
        [Required]
        public Product Product { get; set; } = null!;
        public required string MetaTitle { get; set; }
        public string? MetaDescription { get; set; }
        public string? Slug {  get; set; }
        public string? OpenGraphImageUrl { get; set; }
    }
}
