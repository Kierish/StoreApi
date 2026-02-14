using System.ComponentModel.DataAnnotations;

namespace StoreApi.DTOs
{
    public class ProductReadDto
    {
        public int Id { get; init; }
        public required string Name { get; init; }
        public required string Category { get; init; }
        public decimal Price { get; init; }
    }
}
