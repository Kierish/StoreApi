using System.ComponentModel.DataAnnotations;

namespace StoreApi.DTOs
{
    public record ProductReadDto(
        int Id,
        string Name,
        List<string>? TagName,
        int CategoryId,
        string? CategoryName,
        decimal Price,
        ProductSeoReadDto? ProductSeo
    );

    public record ProductCreateDto(
        string Name,
        List<int>? TagIds,
        int CategoryId,
        decimal? Price,
        ProductSeoCreateDto? ProductSeo
    );

    public record ProductUpdateDto(
        string? Name,
        List<int>? TagIds,
        int? CategoryId,
        decimal? Price,
        ProductSeoUpdateDto? ProductSeo
    );
}
