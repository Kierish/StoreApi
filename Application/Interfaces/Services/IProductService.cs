using Application.DTOs.Products;
using Application.Pagination;
using Domain.Results;

namespace Application.Interfaces.Services
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
