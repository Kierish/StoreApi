namespace Domain.Models.Products
{
    public class Tag
    {
        public Guid Id { get; set; } = Guid.CreateVersion7();
        public required string Name { get; set; }
        public ICollection<Product> Products { get; set; } = new HashSet<Product>();       
    }
}
