using Application.DTOs.Products;
using Application.Interfaces.Services;
using Application.Mappers.Products;
using Application.Pagination;
using Application.Repositories;

using Domain.Constants;
using Domain.ErrorMessages;
using Domain.Models.Products;
using Domain.Results;

namespace Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repo;
        private readonly ICacheService _cache;

        public ProductService(IProductRepository repo, 
            ICacheService cache)
        {
            _repo = repo;
            _cache = cache;
        }

        public async Task<PagedList<ProductReadDto>> GetAllAsync(ProductQueryParameters pageParameters)
        {
            var getAllAsync = async () =>
            {
                var pagedProducts = await _repo.GetListProductsPerPageAsync(pageParameters);
                var dtos = pagedProducts.Items.Select(p => p.ToReadDto()).ToList();

                return new PagedList<ProductReadDto>(
                    dtos,
                    pagedProducts.TotalCount,
                    pagedProducts.Page,
                    pagedProducts.PageSize
                );
            };

            if ( pageParameters.TagNames is null &&
                string.IsNullOrEmpty(pageParameters.CategoryName) &&
                !pageParameters.MaxPrice.HasValue &&
                !pageParameters.MinPrice.HasValue )
            {
                int version = await _cache.GetListCurrentVersionAsync(CacheKeys.GetProductListVersionKey());
                string cacheKey = CacheKeys.GetProductListKey(pageParameters.Page, pageParameters.PageSize, version);
                var cachedResult = await _cache.GetOrCreateAsync<PagedList<ProductReadDto>>(
                    cacheKey,
                    getAllAsync,
                    TimeSpan.FromMinutes(10)
                );

                return cachedResult!;
            }

            var result = await getAllAsync();
            return result!;
        }

        public async Task<Result<ProductReadDto>> GetByIdAsync(Guid id)
        {
            string cacheKey = CacheKeys.GetProductKey(id);
            var dto = await _cache.GetOrCreateAsync<ProductReadDto>(
                cacheKey,
                async () => {
                    var product = await _repo.GetProductByIdAsync(id);
                    return product?.ToReadDto();
                },
                TimeSpan.FromMinutes(10)
            );

            if (dto is null)
                return Result<ProductReadDto>.Failure(ProductErrors.ProductNotFound(id));

            return Result<ProductReadDto>.Success(dto);
        }

        public async Task<Result<ProductReadDto>> CreateAsync(ProductCreateDto dto)
        {
            var categoryId = await _repo.GetCategoryIdAsync(dto.CategoryName);

            if (categoryId is null)
                return Result<ProductReadDto>.Failure(
                    ProductErrors.CategoryNotFound(dto.CategoryName)
                );

            var newProduct = dto.ToEntity(categoryId.Value);

            if (dto.TagNames is not null)
            {
                newProduct.Tags = await _repo.GetTagsContainedInDto(dto.TagNames);
            }

            newProduct.MetaData = dto.PageMetadata?.ToEntity();

            _repo.AddProduct(newProduct);
            await _repo.SaveChangesAsync();

            await _repo.ReferenceCategoryToProduct(newProduct);

            await _cache.IncrementVersionAsync(CacheKeys.GetProductListVersionKey());

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

            if (dto.PageMetadata is { } metaDataDto)
            {
                if (product.MetaData is { } existingMetaData)
                {
                    metaDataDto.MapToEntity(existingMetaData);
                }
                else
                {
                    product.MetaData = metaDataDto.ToEntity();
                }
            }

            await _repo.SaveChangesAsync();

            string cacheKey = CacheKeys.GetProductKey(id);
            await _cache.RemoveCacheAsync(cacheKey);
            await _cache.IncrementVersionAsync(CacheKeys.GetProductListVersionKey());

            return Result<bool>.Success(true);
        }

        public async Task<Result<bool>> DeleteAsync(Guid id)
        {
            var realProduct = await _repo.GetProductByIdAsync(id);

            if (realProduct is null)
                return Result<bool>.Failure(ProductErrors.ProductNotFound(id));

            _repo.RemoveProduct(realProduct);
            await _repo.SaveChangesAsync();

            string cacheKey = CacheKeys.GetProductKey(id);
            await _cache.RemoveCacheAsync(cacheKey);
            await _cache.IncrementVersionAsync(CacheKeys.GetProductListVersionKey());

            return Result<bool>.Success(true);
        }
    }
}
