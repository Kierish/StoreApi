namespace Domain.Entities.Store;

public class Category
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public required string Name { get; set; }
    public PageMetaData? MetaData { get; set; }
    public ICollection<Product> Products { get; set; } = new HashSet<Product>();
}
