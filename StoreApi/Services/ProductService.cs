using StoreApi.Data;
using StoreApi.DTOs;
using StoreApi.Models;
using StoreApi.Mappers;
using StoreApi.Exceptions;
using Microsoft.EntityFrameworkCore;

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

        public List<ProductReadDto> GetAll()
        {
            var products = GetProductWithIncludes();

            return products.Select(p => p.ToReadDto()).ToList();
        }
        public ProductReadDto? GetById(int id)
        {
            var product = GetProductWithIncludes()
                .FirstOrDefault(x => x.Id == id);

            if (product is null)
                throw new NotFoundException($"Product with ID {id} was not found.");

            return product.ToReadDto();
        }
        public ProductReadDto Create(ProductCreateDto dto)
        {
            var newProduct = dto.ToEntity();

            newProduct.Category = _context.Categories
                .Where(c => c.Id == newProduct.CategoryId)
                .FirstOrDefault();

            if (dto.TagIds is not null)
            {
                newProduct.Tags = _context.Tags
                    .Where(t => dto.TagIds.Contains(t.Id))
                    .ToList();
            }

            newProduct.ProductSeo = dto.ProductSeo?.ToEntity();

            _context.Products.Add(newProduct);
            _context.SaveChanges();

            _context.Entry(newProduct)
                .Reference(pr => pr.Category)
                .Load();

            return newProduct.ToReadDto();
        }
        public void Update(int id, ProductUpdateDto dto)
        {
            var product = GetProductWithIncludes()
                .FirstOrDefault(pr => pr.Id == id);

            if (product is null)
                throw new NotFoundException($"Product with ID {id} was not found.");

            dto.ToEntity(product);

            if (dto.TagIds is not null)
            {
                var newTags = _context.Tags
                    .Where(t => dto.TagIds.Contains(t.Id))
                    .ToList();

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

            _context.SaveChanges();
        }
        public void Delete(int id)
        {
            var realProduct = _context.Products.FirstOrDefault(p => p.Id == id);

            if (realProduct is null)
                throw new NotFoundException($"Product with ID {id} was not found.");

            _context.Products.Remove(realProduct);
            _context.SaveChanges();
        }
    }
}
