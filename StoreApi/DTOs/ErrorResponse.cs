using System.ComponentModel.DataAnnotations;

namespace StoreApi.DTOs
{
    public record ErrorResponse(
        int StatusCode,
        string Message,
        string? Details
        );
}
