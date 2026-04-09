using StoreApi.DTOs;
using StoreApi.Helpers;

namespace StoreApi.Interfaces.Services
{
    public interface IProductService 
    {
        Task<PagedList<ProductReadDto>> GetAllAsync(PageParameters pageParameters);
        Task<ProductReadDto?> GetByIdAsync(Guid id);
        Task<ProductReadDto> CreateAsync(ProductCreateDto product);
        Task UpdateAsync(Guid id, ProductUpdateDto product);
        Task DeleteAsync(Guid id);
    }
}
