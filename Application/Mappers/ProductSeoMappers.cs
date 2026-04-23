using Application.DTOs;
using Domain.Entities.Store;

namespace Application.Mappers
{
    public static class ProductSeoMappers
    {
        public static PageMetaData? ToEntity(this PageMetadataCreateDto seoDto)
        {
            if (seoDto == null)
                return null;

            return new PageMetaData
            {
                MetaTitle = seoDto.MetaTitle,
                MetaDescription = seoDto.MetaDescription,
                Slug = seoDto.Slug,
                OpenGraphImageUrl = seoDto.OpenGraphImageUrl,
            };
        }

        public static PageMetaData ToEntity(this PageMetadataUpdateDto seoDto)
        {
            return new PageMetaData
            {
                MetaTitle = seoDto.MetaTitle ?? "",
                MetaDescription = seoDto.MetaDescription,
                Slug = seoDto.Slug,
                OpenGraphImageUrl = seoDto.OpenGraphImageUrl,
            };
        }

        public static void MapToEntity(this PageMetadataUpdateDto seoDto, PageMetaData existingSeo)
        {
            existingSeo.MetaTitle = seoDto.MetaTitle ?? existingSeo.MetaTitle;
            existingSeo.MetaDescription = seoDto.MetaDescription ?? existingSeo.MetaDescription;
            existingSeo.Slug = seoDto.Slug ?? existingSeo.Slug;
            existingSeo.OpenGraphImageUrl =
                seoDto.OpenGraphImageUrl ?? existingSeo.OpenGraphImageUrl;
        }
    }
}
