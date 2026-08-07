using Application.Pagination;
using Application.Repositories;
using Domain.Models.Products;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context) => _context = context;

        private IQueryable<Product> GetProductWithIncludes()
        {
            return _context.Products.Include(pr => pr.Category).Include(pr => pr.Tags);
        }

        public async Task<PagedList<Product>> GetListProductsPerPageAsync(ProductQueryParameters parameters)
        {
            var query = GetProductWithIncludes().AsQueryable();

            if (!string.IsNullOrEmpty(parameters.CategoryName))
            {
                query = query.Where(p => p.Category!.Name == parameters.CategoryName);
            }

            if (parameters.TagNames != null && parameters.TagNames.Any())
            {
                foreach (var tagName in parameters.TagNames)
                {
                    query = query.Where(p => p.Tags!.Any(t => t.Name == tagName));
                }
            }

            if (parameters.MinPrice.HasValue)
            {
                query = query.Where(p => p.Price >= parameters.MinPrice.Value);
            }

            if (parameters.MaxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= parameters.MaxPrice.Value);
            }

            query = query.OrderByDescending(p => p.Id);

            var totalCount = await query.CountAsync();

            var page = parameters.Page;
            var pageSize = parameters.PageSize;

            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PagedList<Product>(items, totalCount, page, pageSize);
        }

        public async Task<Product?> GetProductByIdAsync(Guid id) => 
            await GetProductWithIncludes().FirstOrDefaultAsync(x => x.Id == id);

        public void AddProduct(Product product) => _context.Products.Add(product);

        public async Task<List<Tag>> GetTagsContainedInDto(List<string> tagNames) =>
            await _context.Tags.Where(t => tagNames.Contains(t.Name)).ToListAsync();

        public async Task ReferenceCategoryToProduct(Product product) =>
            await _context.Entry(product).Reference(pr => pr.Category).LoadAsync();

        public async Task<bool> IsTagIdsInDb(List<string> tagNames)
        {
            var count = await _context.Tags.CountAsync(t => tagNames.Contains(t.Name));
            return count == tagNames.Count;
        }

        public void RemoveProduct(Product product)
        {
            product.IsDeleted = true;
            product.DeletedAt = DateTime.UtcNow;
        }

        public async Task<Guid?> GetCategoryIdAsync(string categoryName) =>
            await _context
                .Categories.Where(c => c.Name == categoryName)
                .Select(c => (Guid?)c.Id)
                .FirstOrDefaultAsync();

        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
    }
}
