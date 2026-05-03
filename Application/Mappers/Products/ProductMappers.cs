using Application.DTOs.Products;
using Domain.Models.Products;

namespace Application.Mappers.Products
{
    public static class ProductMappers
    {
        public static Product ToEntity(this ProductCreateDto dto, Guid categoryId)
        {
            return new Product
            {
                Name = dto.Name,
                CategoryId = categoryId,
                Price = dto.Price
            };
        }
        public static void ToEntity(this ProductUpdateDto dto, Product product, Guid? categoryId)
        {
            product.Name = dto.Name ?? product.Name;
            product.CategoryId = categoryId ?? product.CategoryId;
            product.Price = dto.Price.HasValue ? dto.Price.Value : product.Price;
        }
        public static ProductReadDto ToReadDto(this Product dto)
        {
            return new ProductReadDto(
                dto.Id,
                dto.Name,
                dto.Tags?.Select(t => t.Name).ToList() ?? new List<string>(),
                dto.CategoryId,
                dto.Category?.Name,
                dto.Price,
                dto.ProductSeo is not null ? new ProductSeoReadDto(
                    dto.ProductSeo!.MetaTitle,
                    dto.ProductSeo.MetaDescription,
                    dto.ProductSeo.Slug,
                    dto.ProductSeo.OpenGraphImageUrl
                ) : null
            );
        }
    }
}
