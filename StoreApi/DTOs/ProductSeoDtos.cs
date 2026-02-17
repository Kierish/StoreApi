using StoreApi.Models;

namespace StoreApi.DTOs
{
    public record ProductSeoReadDto(
        string MetaTitle,
        string? MetaDescription,
        string? Slug,
        string? OpenGraphImageUrl
    );
    public record ProductSeoCreateDto(
        string MetaTitle,
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
