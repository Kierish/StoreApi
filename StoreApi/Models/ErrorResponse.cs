namespace StoreApi.Models
{
    public class ErrorResponse
    {
        public required int StatusCode { get; set; }
        public required string Message { get; set; }
        public string? Details { get; set; }
    }
}
