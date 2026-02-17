using StoreApi.Models;
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
        [Required(ErrorMessage = "Name is required")][StringLength(100, MinimumLength = 3)] string Name,
        List<int>? TagIds,
        [Required][Range(1, int.MaxValue)] int CategoryId,
        [Required] decimal? Price,
        ProductSeoCreateDto? ProductSeo
    );

    public record ProductUpdateDto(
        [StringLength(100, MinimumLength = 3)] string? Name,
        List<int>? TagIds,
        [Range(1, int.MaxValue)] int? CategoryId,
        [Range(0.1, (double)decimal.MaxValue)] decimal? Price,
        ProductSeoUpdateDto? ProductSeo
    );
}
