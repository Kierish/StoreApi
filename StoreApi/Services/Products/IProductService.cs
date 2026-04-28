using StoreApi.Common.Pagination;
using StoreApi.Common.Primitives;
using StoreApi.DTOs.Products;

namespace StoreApi.Services.Products
{
    public interface IProductService 
    {
        Task<PagedList<ProductReadDto>> GetAllAsync(PageParameters pageParameters);
        Task<Result<ProductReadDto>> GetByIdAsync(Guid id);
        Task<Result<ProductReadDto>> CreateAsync(ProductCreateDto product);
        Task<Result<bool>> UpdateAsync(Guid id, ProductUpdateDto product);
        Task<Result<bool>> DeleteAsync(Guid id);
    }
}
