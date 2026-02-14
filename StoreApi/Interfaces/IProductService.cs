using StoreApi.DTOs;
using StoreApi.Models;

namespace StoreApi.Services
{
    public interface IProductService 
    {
        List<ProductReadDto> GetAll();
        ProductReadDto? GetById(int id);
        ProductReadDto Create(ProductCreateDto product);
        void Update(int id, ProductUpdateDto product);
        void Delete(int id);
    }
}
