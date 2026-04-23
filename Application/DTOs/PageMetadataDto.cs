namespace Application.DTOs;

public record MetadataDto(
    int Id, // Read DTOs usually have Ids
    string MetaTitle,
    string? MetaDescription,
    string? Slug,
    string? OpenGraphImageUrl
);

public record MetadataCreateDto(
    string MetaTitle,
    string? MetaDescription,
    string? Slug,
    string? OpenGraphImageUrl
);

public record MetadataUpdateDto(
    string? MetaTitle,
    string? MetaDescription,
    string? Slug,
    string? OpenGraphImageUrl
);
