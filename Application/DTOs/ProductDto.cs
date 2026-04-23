namespace Application.DTOs;

public record ProductReadDto(
    Guid Id,
    string Name,
    List<string>? TagNames,
    Guid CategoryId,
    string? CategoryName,
    decimal Price,
    PageMetadataDto? ProductSeo
);

public record ProductCreateDto(
    string Name,
    List<string>? TagNames,
    string CategoryName,
    decimal Price,
    PageMetadataCreateDto? Metadata
);

public record ProductUpdateDto(
    string? Name,
    List<string>? TagNames,
    string? CategoryName,
    decimal? Price,
    PageMetadataUpdateDto? Metadata
);
