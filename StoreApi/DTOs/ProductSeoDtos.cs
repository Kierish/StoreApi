using System.ComponentModel.DataAnnotations;

namespace StoreApi.DTOs
{
    public record ProductSeoReadDto(
        string MetaTitle,
        string? MetaDescription,
        string? Slug,
        string? OpenGraphImageUrl
    );
    public record ProductSeoCreateDto(
        [Required] string MetaTitle,
        string? MetaDescription,
        string? Slug,
        string? OpenGraphImageUrl
    );
    public record ProductSeoUpdateDto(
        string? MetaTitle,
        string? MetaDescription,
        string? Slug,
        string? OpenGraphImageUrl
    );
}
