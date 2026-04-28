using System.ComponentModel.DataAnnotations;

namespace StoreApi.DTOs.Products
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
