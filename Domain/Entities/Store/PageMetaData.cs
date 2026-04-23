namespace Domain.Entities.Store;

public class PageMetaData
{
    public required string MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
    public string? Slug { get; set; }
    public string? OpenGraphImageUrl { get; set; }
}
