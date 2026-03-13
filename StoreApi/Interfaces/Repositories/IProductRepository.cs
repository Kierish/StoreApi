using StoreApi.Models.Store;

namespace StoreApi.Interfaces.Repositories
{
    public interface IProductRepository
    {
        Task<List<Product>> GetListAllProductsAsync();
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
