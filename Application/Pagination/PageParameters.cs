namespace Application.Pagination
{
    public record PageParameters
    {
        public int Page { get; init; }
        public int PageSize { get; init; } = 5;
    }
}
