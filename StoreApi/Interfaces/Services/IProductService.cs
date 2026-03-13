using StoreApi.DTOs;

namespace StoreApi.Interfaces.Services
{
    public interface IProductService 
    {
        Task<List<ProductReadDto>> GetAllAsync();
        Task<ProductReadDto?> GetByIdAsync(Guid id);
        Task<ProductReadDto> CreateAsync(ProductCreateDto product);
        Task UpdateAsync(Guid id, ProductUpdateDto product);
        Task DeleteAsync(Guid id);
    }
}
