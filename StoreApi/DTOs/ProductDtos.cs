using System.ComponentModel.DataAnnotations;

namespace StoreApi.DTOs
{
    public record ProductReadDto(
        Guid Id,
        string Name,
        List<string>? TagNames,
        Guid CategoryId,
        string? CategoryName,
        decimal Price,
        ProductSeoReadDto? ProductSeo
    );

    public record ProductCreateDto(
        string Name,
        List<string>? TagNames,
        string CategoryName,
        decimal Price,
        ProductSeoCreateDto? ProductSeo
    );

    public record ProductUpdateDto(
        string? Name,
        List<string>? TagNames,
        string? CategoryName,
        decimal? Price,
        ProductSeoUpdateDto? ProductSeo
    );
}
