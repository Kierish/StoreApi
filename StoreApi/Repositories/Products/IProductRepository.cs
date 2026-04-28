using StoreApi.Common.Pagination;
using StoreApi.Models.Store;

namespace StoreApi.Repositories.Products
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
