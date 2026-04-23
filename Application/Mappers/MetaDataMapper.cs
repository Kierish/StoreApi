using Application.DTOs;
using Domain.Entities.Store;

namespace Application.Mappers;

public static class MetaDataMapper
{
    public static PageMetadataDto ToMetadataDto(this PageMetaData pageMetaData)
    {
        if (pageMetaData == null)
            return null;

        return new PageMetadataDto(
            pageMetaData.MetaTitle,
            pageMetaData.MetaDescription,
            pageMetaData.Slug,
            pageMetaData.OpenGraphImageUrl
        );
    }
}
