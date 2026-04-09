namespace StoreApi.DTOs
{
    public class PageParameters
    {
        private const int MaxPageSize = 50;
        public int Page { get; set; } = 1;

        private int _pageSize = 2;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
        }
    }
}
