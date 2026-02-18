using StoreApi.DTOs;
using StoreApi.Models;

namespace StoreApi.Services
{
    public interface IProductService 
    {
        Task<List<ProductReadDto>> GetAllAsync();
        Task<ProductReadDto?> GetByIdAsync(int id);
        Task<ProductReadDto> CreateAsync(ProductCreateDto product);
        Task UpdateAsync(int id, ProductUpdateDto product);
        Task DeleteAsync(int id);
    }
}
