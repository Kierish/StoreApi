using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StoreApi.Models.Store
{
    public class Product : ISoftDelete
    {
        public Guid Id { get; set; } = Guid.CreateVersion7();
        public required string Name { get; set; }
        public ICollection<Tag>? Tags { get; set; } = new HashSet<Tag>();
        public required Guid CategoryId { get; set; }   
        public Category? Category { get; set; }
        public required decimal Price { get; set; }
        public ProductSeo? ProductSeo { get; set; }

        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; } 
    }
}
