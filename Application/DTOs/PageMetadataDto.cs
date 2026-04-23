namespace Application.DTOs;

public record PageMetadataDto(
    string MetaTitle,
    string? MetaDescription,
    string? Slug,
    string? OpenGraphImageUrl
);

public record PageMetadataCreateDto(
    string MetaTitle,
    string? MetaDescription,
    string? Slug,
    string? OpenGraphImageUrl
);

public record PageMetadataUpdateDto(
    string? MetaTitle,
    string? MetaDescription,
    string? Slug,
    string? OpenGraphImageUrl
);
