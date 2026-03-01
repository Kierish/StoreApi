using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StoreApi.Models.Store
{
    public class Product
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public ICollection<Tag>? Tags { get; set; } = new HashSet<Tag>();
        public required int CategoryId { get; set; }    
        public Category? Category { get; set; }
        [Range(0.1, (double)decimal.MaxValue)]
        public required decimal Price { get; set; }
        public ProductSeo? ProductSeo { get; set; }
    }
}
