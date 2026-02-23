using StoreApi.DTOs;
using StoreApi.Models.Store;

namespace StoreApi.Mappers
{
    public static class ProductSeoMappers
    {
        public static ProductSeo? ToEntity(this ProductSeoCreateDto seoDto)
        {
            if (seoDto == null) return null;

            return new ProductSeo
            {
                MetaTitle = seoDto.MetaTitle,
                MetaDescription = seoDto.MetaDescription,
                Slug = seoDto.Slug,
                OpenGraphImageUrl = seoDto.OpenGraphImageUrl
            };
        }
        public static ProductSeo ToEntity(this ProductSeoUpdateDto seoDto)
        {
            return new ProductSeo
            {
                MetaTitle = seoDto.MetaTitle ?? "",
                MetaDescription = seoDto.MetaDescription,
                Slug = seoDto.Slug,
                OpenGraphImageUrl = seoDto.OpenGraphImageUrl
            };
        }
        public static void MapToEntity(this ProductSeoUpdateDto seoDto, ProductSeo existingSeo)
        {
            existingSeo.MetaTitle = seoDto.MetaTitle ?? existingSeo.MetaTitle;
            existingSeo.MetaDescription = seoDto.MetaDescription ?? existingSeo.MetaDescription;
            existingSeo.Slug = seoDto.Slug ?? existingSeo.Slug;
            existingSeo.OpenGraphImageUrl = seoDto.OpenGraphImageUrl ?? existingSeo.OpenGraphImageUrl;
        }
    }
}
