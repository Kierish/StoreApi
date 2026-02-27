using Microsoft.EntityFrameworkCore;
using StoreApi.Data;
using StoreApi.Interfaces.Repositories;
using StoreApi.Models.Store;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Threading.Tasks;

namespace StoreApi.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;
        public ProductRepository(AppDbContext context) => _context = context;

        private IQueryable<Product> GetProductWithIncludes()
        {
            return _context.Products
                .Include(pr => pr.Category)
                .Include(pr => pr.Tags)
                .Include(pr => pr.ProductSeo);
        }

        public async Task<List<Product>> GetListAllProductsAsync()
            => await GetProductWithIncludes()
            .ToListAsync();

        public async Task<Product?> GetProductByIdAsync(int id)
            => await GetProductWithIncludes()
            .FirstOrDefaultAsync(x => x.Id == id);

        public void AddProduct(Product product)
            => _context.Products.Add(product);

        public async Task<List<Tag>> GetTagsContainedInDto(List<int> tags)
            => await _context.Tags
            .Where(t => tags.Contains(t.Id))
            .ToListAsync();

        public async Task ReferenceCategoryToProduct(Product product)
            => await _context.Entry(product)
            .Reference(pr => pr.Category)
            .LoadAsync();

        public async Task<bool> IsTagIdsInDb(List<int> tagIds)
        {
            var count = await _context.Tags.CountAsync(t => tagIds.Contains(t.Id));
            return count == tagIds.Count;
        }

        public void RemoveProduct(Product product)
            => _context.Products.Remove(product);

        public async Task SaveChangesAsync()
            => await _context.SaveChangesAsync();
    }
}
