using Microsoft.EntityFrameworkCore;
using StoreApi.Data;
using StoreApi.DTOs;
using StoreApi.Exceptions;
using StoreApi.Helpers;
using StoreApi.Interfaces.Repositories;
using StoreApi.Interfaces.Services;
using StoreApi.Mappers;
using StoreApi.Models.Store;
using System.Security.Cryptography;

namespace StoreApi.Services
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
        public async Task<ProductReadDto?> GetByIdAsync(Guid id)
        {
            var product = await _repo.GetProductByIdAsync(id);

            if (product is null)
                throw new NotFoundException($"Product with ID {id} was not found.");

            return product.ToReadDto();
        }
        public async Task<ProductReadDto> CreateAsync(ProductCreateDto dto)
        {
            var categoryId = await _repo.GetCategoryIdAsync(dto.CategoryName);

            if (categoryId is null)
                throw new NotFoundException($"Category {dto.CategoryName} doesn't exist.");

            var newProduct = dto.ToEntity(categoryId.Value);

            if (dto.TagNames is not null)
            {
                newProduct.Tags = await _repo.GetTagsContainedInDto(dto.TagNames);
            }

            newProduct.ProductSeo = dto.ProductSeo?.ToEntity();

            _repo.AddProduct(newProduct);
            await _repo.SaveChangesAsync();

            await _repo.ReferenceCategoryToProduct(newProduct);

            return newProduct.ToReadDto();
        }
        public async Task UpdateAsync(Guid id, ProductUpdateDto dto)
        {
            var product = await _repo.GetProductByIdAsync(id);

            if (product is null)
                throw new NotFoundException($"Product with ID {id} was not found.");

            Guid? categoryId = null;

            if (dto.CategoryName is not null)
            {
                categoryId = await _repo.GetCategoryIdAsync(dto.CategoryName);

                if (categoryId is null)
                    throw new NotFoundException($"Category {dto.CategoryName} doesn't exist.");
            }

            dto.ToEntity(product, categoryId);

            if (dto.TagNames is not null)
            {
                if (!await _repo.IsTagIdsInDb(dto.TagNames))
                    throw new NotFoundException("Such tags do not exists.");

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
        }
        public async Task DeleteAsync(Guid id)
        {
            var realProduct = await _repo.GetProductByIdAsync(id);

            if (realProduct is null)
                throw new NotFoundException($"Product with ID {id} was not found.");

            _repo.RemoveProduct(realProduct);
            await _repo.SaveChangesAsync();
        }
    }
}
