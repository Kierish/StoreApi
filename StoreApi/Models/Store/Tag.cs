namespace StoreApi.Models.Store
{
    public class Tag
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public List<Product>? Products { get; set; }
    }
}
