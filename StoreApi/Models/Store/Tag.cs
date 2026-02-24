namespace StoreApi.Models.Store
{
    public class Tag
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public ICollection<Product>? Products { get; set; } = new HashSet<Product>();       
    }
}
