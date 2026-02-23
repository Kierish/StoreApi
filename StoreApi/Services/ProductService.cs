using StoreApi.Data;
using StoreApi.DTOs;
using StoreApi.Mappers;
using StoreApi.Exceptions;
using Microsoft.EntityFrameworkCore;
using StoreApi.Models.Store;

namespace StoreApi.Services
{
    public class ProductService : IProductService
    {
        private readonly AppDbContext _context;

        public ProductService(AppDbContext context)
        {
            _context = context;
        }
        
        private IQueryable<Product> GetProductWithIncludes()
        {
            return _context.Products
                .Include(pr => pr.Category)
                .Include(pr => pr.Tags)
                .Include(pr => pr.ProductSeo);
        }

        public async Task<List<ProductReadDto>> GetAllAsync()
        {
            var products = await GetProductWithIncludes().ToListAsync();

            return products.Select(p => p.ToReadDto()).ToList();
        }
        public async Task<ProductReadDto?> GetByIdAsync(int id)
        {
            var product = await GetProductWithIncludes()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (product is null)
                throw new NotFoundException($"Product with ID {id} was not found.");

            return product.ToReadDto();
        }
        public async Task<ProductReadDto> CreateAsync(ProductCreateDto dto)
        {
            var newProduct = dto.ToEntity();

            if (dto.TagIds is not null)
            {
                newProduct.Tags = await _context.Tags
                    .Where(t => dto.TagIds.Contains(t.Id))
                    .ToListAsync();
            }

            newProduct.ProductSeo = dto.ProductSeo?.ToEntity();

            _context.Products.Add(newProduct);
            await _context.SaveChangesAsync();

            await _context.Entry(newProduct)
                .Reference(pr => pr.Category)
                .LoadAsync();

            return newProduct.ToReadDto();
        }
        public async Task UpdateAsync(int id, ProductUpdateDto dto)
        {
            var product = await GetProductWithIncludes()
                .FirstOrDefaultAsync(pr => pr.Id == id);

            if (product is null)
                throw new NotFoundException($"Product with ID {id} was not found.");

            dto.ToEntity(product);

            if (dto.TagIds is not null)
            {
                var newTags = await _context.Tags
                    .Where(t => dto.TagIds.Contains(t.Id))
                    .ToListAsync();

                product.Tags?.Clear();

                product.Tags?.AddRange(newTags);
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

            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id)
        {
            var realProduct = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);

            if (realProduct is null)
                throw new NotFoundException($"Product with ID {id} was not found.");

            _context.Products.Remove(realProduct);
            await _context.SaveChangesAsync();
        }
    }
}
