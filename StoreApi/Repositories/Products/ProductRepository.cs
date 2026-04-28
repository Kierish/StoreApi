using Microsoft.EntityFrameworkCore;
using StoreApi.Common.Pagination;
using StoreApi.Data;
using StoreApi.Models.Store;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Threading.Tasks;

namespace StoreApi.Repositories.Products
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;
        public ProductRepository(AppDbContext context) => _context = context;

        private IQueryable<Product> GetProductWithIncludes()
        {
            return _context.Products
                .Include(pr => pr.Category)
                .Include(pr => pr.Tags);
        }

        public async Task<PagedList<Product>> GetListProductsPerPageAsync(PageParameters parameters)
        {
            var query = GetProductWithIncludes();
            return await PagedList<Product>.CreateAsync(query, parameters.Page, parameters.PageSize);
        }

        public async Task<Product?> GetProductByIdAsync(Guid id)
            => await GetProductWithIncludes()
            .FirstOrDefaultAsync(x => x.Id == id);

        public void AddProduct(Product product)
            => _context.Products.Add(product);

        public async Task<List<Tag>> GetTagsContainedInDto(List<string> tagNames)
            => await _context.Tags
            .Where(t => tagNames.Contains(t.Name))
            .ToListAsync();

        public async Task ReferenceCategoryToProduct(Product product)
            => await _context.Entry(product)
            .Reference(pr => pr.Category)
            .LoadAsync();

        public async Task<bool> IsTagIdsInDb(List<string> tagNames)
        {
            var count = await _context.Tags.CountAsync(t => tagNames.Contains(t.Name));
            return count == tagNames.Count;
        }

        public void RemoveProduct(Product product)
            => _context.Products.Remove(product);

        public async Task<Guid?> GetCategoryIdAsync(string categoryName) 
            => await _context.Categories
            .Where(c => c.Name == categoryName)
            .Select(c => (Guid?)c.Id)
            .FirstOrDefaultAsync();

        public async Task SaveChangesAsync()
            => await _context.SaveChangesAsync();
    }
}
