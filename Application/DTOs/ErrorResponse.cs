namespace Application.DTOs;

public record ErrorResponse(int StatusCode, string Message, string? Details);
