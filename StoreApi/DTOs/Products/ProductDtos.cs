using System.ComponentModel.DataAnnotations;

namespace StoreApi.DTOs.Products
{
    /// <summary>
    /// Represents a product returned by the API.
    /// </summary>
    /// <param name="Id">The unique identifier of the product.</param>
    /// <param name="Name">The name of the product.</param>
    /// <param name="TagNames">The tags associated with the product.</param>
    /// <param name="CategoryId">The identifier of the category this product belongs to.</param>
    /// <param name="CategoryName">The name of the associated category.</param>
    /// <param name="Price">The price of the product.</param>
    /// <param name="ProductSeo">The SEO metadata for the product.</param>
    public record ProductReadDto(
        Guid Id,
        string Name,
        List<string>? TagNames,
        Guid CategoryId,
        string? CategoryName,
        decimal Price,
        ProductSeoReadDto? ProductSeo
    );

    /// <summary>
    /// Contains data to update an existing product.
    /// </summary>
    /// <param name="Name">The updated name of the product.</param>
    /// <param name="TagNames">The updated tags for the product.</param>
    /// <param name="CategoryName">The updated category for the product.</param>
    /// <param name="Price">The updated price.</param>
    /// <param name="ProductSeo">The updated SEO metadata.</param>
    public record ProductCreateDto(
        string Name,
        List<string>? TagNames,
        string CategoryName,
        decimal Price,
        ProductSeoCreateDto? ProductSeo
    );

    /// <summary>
    /// The payload required to update an existing product. Only provided fields will be modified.
    /// </summary>
    /// <param name="Name">The new commercial display name of the product.</param>
    /// <param name="TagNames">The complete new list of tags. Providing this overrides the existing list entirely.</param>
    /// <param name="CategoryName">The exact system name of the new parent category.</param>
    /// <param name="Price">The updated base retail price in the store's default currency.</param>
    /// <param name="ProductSeo">Specific SEO metadata fields to update.</param>
    public record ProductUpdateDto(
        string? Name,
        List<string>? TagNames,
        string? CategoryName,
        decimal? Price,
        ProductSeoUpdateDto? ProductSeo
    );
}
