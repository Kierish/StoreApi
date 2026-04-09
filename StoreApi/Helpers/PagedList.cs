using Microsoft.EntityFrameworkCore;

namespace StoreApi.Helpers
{
    public class PagedList<T>
    {
        public IReadOnlyList<T> Items { get; }
        public int Page { get; }
        public int PageSize { get; }
        public int TotalCount { get; }
        public bool HasNextPage => Page * PageSize < TotalCount;
        public bool HasPreviousPage => Page > 1;

        public PagedList(IReadOnlyList<T> items, int count, int pageNumber, int pageSize)
        {
            Items = items;
            TotalCount = count;
            PageSize = pageSize;
            Page = pageNumber;
        }

        public static async Task<PagedList<T>> CreateAsync(IQueryable<T> query, int page, int pageSize)
        {
            var validPage = page <= 0 ? 1 : page;
            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((validPage - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return new PagedList<T>(items, totalCount, validPage, pageSize);
        }
    }
}
