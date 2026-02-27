using StoreApi.Data;
using StoreApi.DTOs;
using StoreApi.Mappers;
using StoreApi.Exceptions;
using StoreApi.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using StoreApi.Models.Store;
using StoreApi.Interfaces.Services;

namespace StoreApi.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repo;

        public ProductService(IProductRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<ProductReadDto>> GetAllAsync()
        {
            var products = await _repo.GetListAllProductsAsync();

            return products.Select(p => p.ToReadDto()).ToList();
        }
        public async Task<ProductReadDto?> GetByIdAsync(int id)
        {
            var product = await _repo.GetProductByIdAsync(id);

            if (product is null)
                throw new NotFoundException($"Product with ID {id} was not found.");

            return product.ToReadDto();
        }
        public async Task<ProductReadDto> CreateAsync(ProductCreateDto dto)
        {
            var newProduct = dto.ToEntity();

            if (dto.TagIds is not null)
            {
                newProduct.Tags = await _repo.GetTagsContainedInDto(dto.TagIds);
            }

            newProduct.ProductSeo = dto.ProductSeo?.ToEntity();

            _repo.AddProduct(newProduct);
            await _repo.SaveChangesAsync();

            await _repo.ReferenceCategoryToProduct(newProduct);

            return newProduct.ToReadDto();
        }
        public async Task UpdateAsync(int id, ProductUpdateDto dto)
        {
            var product = await _repo.GetProductByIdAsync(id);

            if (product is null)
                throw new NotFoundException($"Product with ID {id} was not found.");

            dto.ToEntity(product);

            if (dto.TagIds is not null)
            {
                if (!await _repo.IsTagIdsInDb(dto.TagIds))
                    throw new NotFoundException("Such tags do not exists.");

                var newTags = await _repo.GetTagsContainedInDto(dto.TagIds);

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
        public async Task DeleteAsync(int id)
        {
            var realProduct = await _repo.GetProductByIdAsync(id);

            if (realProduct is null)
                throw new NotFoundException($"Product with ID {id} was not found.");

            _repo.RemoveProduct(realProduct);
            await _repo.SaveChangesAsync();
        }
    }
}
