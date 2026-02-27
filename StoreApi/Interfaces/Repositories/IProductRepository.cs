using StoreApi.Models.Store;

namespace StoreApi.Interfaces.Repositories
{
    public interface IProductRepository
    {
        Task<List<Product>> GetListAllProductsAsync();
        Task<Product?> GetProductByIdAsync(int id);
        void AddProduct(Product product);
        Task<List<Tag>> GetTagsContainedInDto(List<int> tags);
        Task ReferenceCategoryToProduct(Product product);
        Task<bool> IsTagIdsInDb(List<int> tagIds);
        void RemoveProduct(Product product);
        Task SaveChangesAsync();
    }
}
