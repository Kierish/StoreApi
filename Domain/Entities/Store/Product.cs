namespace Domain.Entities.Store;

public class Product
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public required string Name { get; set; }
    public ICollection<Tag>? Tags { get; set; } = new HashSet<Tag>();
    public required Guid CategoryId { get; set; }
    public Category? Category { get; set; }
    public required decimal Price { get; set; }
    public PageMetaData? MetaData { get; set; }
}
