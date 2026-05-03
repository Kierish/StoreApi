using Application.Pagination;
using Domain.Models.Products;

namespace Application.Repositories
{
    public interface IProductRepository
    {
        Task<PagedList<Product>> GetListProductsPerPageAsync(PageParameters parameters);
        Task<Product?> GetProductByIdAsync(Guid id);
        void AddProduct(Product product);
        Task<List<Tag>> GetTagsContainedInDto(List<string> tagNames);
        Task ReferenceCategoryToProduct(Product product);
        Task<bool> IsTagIdsInDb(List<string> tagNames);
        void RemoveProduct(Product product);
        Task<Guid?> GetCategoryIdAsync(string categoryName);
        Task SaveChangesAsync();
    }
}
