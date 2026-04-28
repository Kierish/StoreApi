using StoreApi.Common.ErrorMessages;
using StoreApi.Common.Mappers.Products;
using StoreApi.Common.Pagination;
using StoreApi.Common.Primitives;
using StoreApi.DTOs.Products;
using StoreApi.Repositories.Products;

namespace StoreApi.Services.Products
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repo;
        public ProductService(IProductRepository repo)
        {
            _repo = repo;
        }

        public async Task<PagedList<ProductReadDto>> GetAllAsync(PageParameters pageParameters)
        {   
            var pagedProducts = await _repo.GetListProductsPerPageAsync(pageParameters);
            
            var dtos = pagedProducts.Items.Select(p => p.ToReadDto()).ToList();

            return new PagedList<ProductReadDto>(
                dtos,
                pagedProducts.TotalCount,
                pagedProducts.Page,
                pagedProducts.PageSize);
        }
        public async Task<Result<ProductReadDto>> GetByIdAsync(Guid id)
        {
            var product = await _repo.GetProductByIdAsync(id);

            if (product is null)
                return Result<ProductReadDto>.Failure(ProductErrors.ProductNotFound(id));

            return Result<ProductReadDto>.Success(product.ToReadDto());
        }   
        public async Task<Result<ProductReadDto>> CreateAsync(ProductCreateDto dto)
        {
            var categoryId = await _repo.GetCategoryIdAsync(dto.CategoryName);

            if (categoryId is null)
                return Result<ProductReadDto>.Failure(ProductErrors.CategoryNotFound(dto.CategoryName));

            var newProduct = dto.ToEntity(categoryId.Value);

            if (dto.TagNames is not null)
            {
                newProduct.Tags = await _repo.GetTagsContainedInDto(dto.TagNames);
            }

            newProduct.ProductSeo = dto.ProductSeo?.ToEntity();

            _repo.AddProduct(newProduct);
            await _repo.SaveChangesAsync();

            await _repo.ReferenceCategoryToProduct(newProduct);

            return Result<ProductReadDto>.Success(newProduct.ToReadDto());
        }
        public async Task<Result<bool>> UpdateAsync(Guid id, ProductUpdateDto dto)
        {
            var product = await _repo.GetProductByIdAsync(id);

            if (product is null)
                return Result<bool>.Failure(ProductErrors.ProductNotFound(id));

            Guid? categoryId = null;

            if (dto.CategoryName is not null)
            {
                categoryId = await _repo.GetCategoryIdAsync(dto.CategoryName);

                if (categoryId is null)
                    return Result<bool>.Failure(ProductErrors.CategoryNotFound(dto.CategoryName));
            }

            dto.ToEntity(product, categoryId);

            if (dto.TagNames is not null)
            {
                if (!await _repo.IsTagIdsInDb(dto.TagNames))
                    return Result<bool>.Failure(ProductErrors.TagsNotFound());

                var newTags = await _repo.GetTagsContainedInDto(dto.TagNames);

                product.Tags?.Clear();

                foreach (var tag in newTags)
                {
                    product.Tags?.Add(tag);
                }
            }

            if (dto.ProductSeo is { } seoDto)
            {
                if (product.ProductSeo is { } existingSeo)
                {
                    seoDto.MapToEntity(existingSeo);
                }
                else
                {
                    product.ProductSeo = seoDto.ToEntity();
                }
            }

            await _repo.SaveChangesAsync();

            return Result<bool>.Success(true);
        }
        public async Task<Result<bool>> DeleteAsync(Guid id)
        {
            var realProduct = await _repo.GetProductByIdAsync(id);

            if (realProduct is null)
                return Result<bool>.Failure(ProductErrors.ProductNotFound(id));

            _repo.RemoveProduct(realProduct);
            await _repo.SaveChangesAsync();

            return Result<bool>.Success(true);
        }
    }
}
